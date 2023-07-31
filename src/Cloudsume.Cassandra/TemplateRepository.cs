namespace Cloudsume.Cassandra;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Cloudsume.Resume;
using Cloudsume.Template;
using Cornot;
using global::Cassandra;
using global::Cassandra.Mapping;
using NetUlid;
using Ultima.Extensions.Currency;

public sealed class TemplateRepository : ITemplateRepository
{
    private readonly IMapperFactory db;
    private readonly IReadConsistencyProvider readConsistencies;

    public TemplateRepository(IMapperFactory db, IReadConsistencyProvider readConsistencies)
    {
        this.db = db;
        this.readConsistencies = readConsistencies;
    }

    public async Task<int> CountRegistrationAsync(Guid ownerId, CancellationToken cancellationToken = default)
    {
        var db = this.db.CreateMapper();

        return await db.FirstAsync<int>("SELECT COUNT(*) FROM template_registrations_by_owner_id WHERE owner_id = ?", ownerId);
    }

    public async Task<int> CountTemplateAsync(Guid registrationId, CancellationToken cancellationToken = default)
    {
        var db = this.db.CreateMapper();

        return await db.FirstAsync<int>("SELECT COUNT(*) FROM templates_by_registration WHERE registration_id = ?", registrationId);
    }

    public async Task CreateTemplateAsync(Template template, CancellationToken cancellationToken = default)
    {
        if (template.ResumeCount != 0)
        {
            throw new ArgumentException($"{nameof(template.ResumeCount)} is not zero.", nameof(template));
        }

        var db = this.db.CreateMapper();
        var row = new Models.Template()
        {
            Id = template.Id.ToByteArray(),
            RegistrationId = template.RegistrationId,
            ApplicableData = template.ApplicableData,
            ExperienceOptions = template.RenderOptions.GetExperienceOptions(),
            EducationOptions = template.RenderOptions.GetEducationOptions(),
            SkillOptions = template.RenderOptions.GetSkillOptions(),
            Category = Convert.ToSByte(template.Category),
            ReleaseNote = template.ReleaseNote.NullOnEmpty(),
        };

        await db.InsertAsync(row, CqlQueryOptions.New().SetConsistencyLevel(ConsistencyLevel.EachQuorum));
    }

    public async Task DecreaseResumeCountAsync(Template template, CancellationToken cancellationToken = default)
    {
        var db = this.db.CreateMapper();

        await db.UpdateAsync<Models.TemplateStats>("SET resume_count = resume_count - 1 WHERE template_id = ?", template.Id.ToByteArray());
        await db.UpdateAsync<Models.TemplateRegistrationStats>("SET resume_count = resume_count - 1 WHERE registration_id = ?", template.RegistrationId);
    }

    public async Task<TemplateRegistration?> GetRegistrationAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var db = this.db.CreateMapper();
        var query = Cql.New("WHERE id = ?", id).WithOptions(options => options.SetConsistencyLevel(this.readConsistencies.StrongConsistency));
        var row = await db.FirstOrDefaultAsync<Models.TemplateRegistration>(query);

        if (row == null)
        {
            return null;
        }

        return await this.ToDomainAsync(row);
    }

    public async Task<CultureInfo?> GetRegistrationCultureAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var db = this.db.CreateMapper();

        // We don't need strong consistency here due to this method always get called once the registration known to exists.
        var language = await db.FirstOrDefaultAsync<string>("SELECT language FROM template_registrations WHERE id = ?", id);

        if (language == null)
        {
            return null;
        }

        return CultureInfo.GetCultureInfo(language);
    }

    public async Task<Template?> GetTemplateAsync(Ulid id, CancellationToken cancellationToken = default)
    {
        var db = this.db.CreateMapper();
        var query = Cql.New("WHERE id = ?", id.ToByteArray()).WithOptions(options => options.SetConsistencyLevel(this.readConsistencies.StrongConsistency));
        var row = await db.FirstOrDefaultAsync<Models.Template>(query);

        return row != null ? await this.ToDomainAsync(row) : null;
    }

    public async Task IncreaseResumeCountAsync(Template template, CancellationToken cancellationToken = default)
    {
        var db = this.db.CreateMapper();

        await db.UpdateAsync<Models.TemplateStats>("SET resume_count = resume_count + 1 WHERE template_id = ?", template.Id.ToByteArray());
        await db.UpdateAsync<Models.TemplateRegistrationStats>("SET resume_count = resume_count + 1 WHERE registration_id = ?", template.RegistrationId);
    }

    public async Task<IEnumerable<TemplateRegistration>> ListRegistrationAsync(CancellationToken cancellationToken = default)
    {
        var db = this.db.CreateMapper();
        var domains = new List<TemplateRegistration>();
        var options = CqlQueryOptions.New().SetConsistencyLevel(this.readConsistencies.StrongConsistency);

        foreach (var row in await db.FetchAsync<Models.TemplateRegistration>(options))
        {
            domains.Add(await this.ToDomainAsync(row));
        }

        return domains;
    }

    public async Task<IEnumerable<TemplateRegistration>> ListRegistrationByOwnerAsync(Guid ownerId, CancellationToken cancellationToken = default)
    {
        var db = this.db.CreateMapper();
        var domains = new List<TemplateRegistration>();
        var query = Cql.New("FROM template_registrations_by_owner_id WHERE owner_id = ?", ownerId).WithOptions(options =>
        {
            options.SetConsistencyLevel(this.readConsistencies.StrongConsistency);
        });

        foreach (var row in await db.FetchAsync<Models.TemplateRegistration>(query))
        {
            domains.Add(await this.ToDomainAsync(row));
        }

        return domains;
    }

    public async Task<IEnumerable<Template>> ListTemplateAsync(Guid registrationId, CancellationToken cancellationToken = default)
    {
        var db = this.db.CreateMapper();
        var domains = new List<Template>();
        var query = Cql.New("FROM templates_by_registration WHERE registration_id = ? ORDER BY id DESC", registrationId).WithOptions(options =>
        {
            options.SetConsistencyLevel(this.readConsistencies.StrongConsistency);
        });

        foreach (var row in await db.FetchAsync<Models.Template>(query))
        {
            domains.Add(await this.ToDomainAsync(row));
        }

        return domains;
    }

    public async Task RegisterAsync(TemplateRegistration registration, CancellationToken cancellationToken = default)
    {
        if (registration.ResumeCount != 0)
        {
            throw new ArgumentException($"{nameof(registration.ResumeCount)} is not zero.", nameof(registration));
        }

        var db = this.db.CreateMapper();
        var row = new Models.TemplateRegistration()
        {
            Id = registration.Id,
            OwnerId = registration.UserId,
            Name = registration.Name,
            Description = registration.Description,
            Website = registration.Website?.AbsoluteUri,
            Language = registration.Culture.Name,
            ApplicableJobs = registration.ApplicableJobs,
            Category = Convert.ToSByte(registration.Category),
            Prices = registration.Prices.ToCassandra(),
            UnlistedReason = registration.UnlistedReason is { } unlisted ? Convert.ToSByte(unlisted) : null,
            CreatedAt = registration.CreatedAt,
            UpdatedAt = registration.UpdatedAt,
        };

        await db.InsertAsync(row, CqlQueryOptions.New().SetConsistencyLevel(ConsistencyLevel.EachQuorum));
    }

    public Task SetApplicableJobsAsync(Guid id, IEnumerable<Guid> jobs, CancellationToken cancellationToken = default)
    {
        var db = this.db.CreateMapper();
        var cql = Cql.New("SET applicable_jobs = ? WHERE id = ?", jobs, id).WithOptions(options =>
        {
            options.SetConsistencyLevel(ConsistencyLevel.EachQuorum);
        });

        return db.UpdateAsync<Models.TemplateRegistration>(cql);
    }

    public Task SetDescriptionAsync(Guid id, string description, CancellationToken cancellationToken = default)
    {
        var db = this.db.CreateMapper();
        var cql = Cql.New("SET description = ? WHERE id = ?", description, id).WithOptions(options =>
        {
            options.SetConsistencyLevel(ConsistencyLevel.LocalQuorum);
        });

        return db.UpdateAsync<Models.TemplateRegistration>(cql);
    }

    public Task SetNameAsync(Guid id, string name, CancellationToken cancellationToken = default)
    {
        var db = this.db.CreateMapper();
        var cql = Cql.New("SET name = ? WHERE id = ?", name, id).WithOptions(options =>
        {
            options.SetConsistencyLevel(ConsistencyLevel.LocalQuorum);
        });

        return db.UpdateAsync<Models.TemplateRegistration>(cql);
    }

    public Task SetPricesAsync(Guid id, IReadOnlyDictionary<CurrencyCode, decimal> prices, CancellationToken cancellationToken = default)
    {
        var db = this.db.CreateMapper();
        var cql = Cql.New("SET prices = ? WHERE id = ?", prices.ToCassandra(), id).WithOptions(options =>
        {
            options.SetConsistencyLevel(ConsistencyLevel.EachQuorum);
        });

        return db.UpdateAsync<Models.TemplateRegistration>(cql);
    }

    public Task SetRegistrationCategoryAsync(Guid id, RegistrationCategory category, CancellationToken cancellationToken = default)
    {
        var db = this.db.CreateMapper();
        var cql = Cql.New("SET category = ? WHERE id = ?", Convert.ToSByte(category), id).WithOptions(options =>
        {
            options.SetConsistencyLevel(ConsistencyLevel.EachQuorum);
        });

        return db.UpdateAsync<Models.TemplateRegistration>(cql);
    }

    public Task SetUpdatedAsync(Guid id, DateTime time, CancellationToken cancellationToken = default)
    {
        var db = this.db.CreateMapper();

        return db.UpdateAsync<Models.TemplateRegistration>("SET updated_at = ? WHERE id = ?", new DateTimeOffset(time), id);
    }

    public Task SetWebsiteAsync(Guid id, Uri? website, CancellationToken cancellationToken = default)
    {
        var db = this.db.CreateMapper();
        var cql = Cql.New("SET website = ? WHERE id = ?", website?.AbsoluteUri, id).WithOptions(options =>
        {
            options.SetConsistencyLevel(ConsistencyLevel.LocalQuorum);
        });

        return db.UpdateAsync<Models.TemplateRegistration>(cql);
    }

    private async Task<TemplateRegistration> ToDomainAsync(Models.TemplateRegistration row)
    {
        // Sanity checks.
        if (row.Language == null)
        {
            throw new DataCorruptionException(row, "Language is null.");
        }

        var id = row.Id;
        var owner = row.OwnerId;
        var name = row.Name;
        var description = row.Description;
        var website = row.Website != null ? new Uri(row.Website, UriKind.Absolute) : null;
        var culture = CultureInfo.GetCultureInfo(row.Language);
        var applicableJobs = row.ApplicableJobs;
        var category = (RegistrationCategory)row.Category;
        var prices = row.Prices ?? new Dictionary<string, decimal>();
        var unlisted = (UnlistedReason?)row.UnlistedReason;
        var createdAt = row.CreatedAt.LocalDateTime;
        var updatedAt = row.UpdatedAt?.LocalDateTime ?? createdAt;

        if (name == null)
        {
            throw new DataCorruptionException(row, "Name is null.");
        }

        if (description == null)
        {
            throw new DataCorruptionException(row, "Description is null.");
        }

        if (applicableJobs == null)
        {
            throw new DataCorruptionException(row, "ApplicableJobs is null.");
        }

        // Load stats.
        var db = this.db.CreateMapper();
        var stats = await db.FirstOrDefaultAsync<Models.TemplateRegistrationStats>("WHERE registration_id = ?", id);
        long resumeCount;

        if (stats != null)
        {
            resumeCount = stats.ResumeCount;
        }
        else
        {
            resumeCount = 0;
        }

        return new(id, owner, name, description, website, culture, applicableJobs, category, prices.ToPriceList(), resumeCount, unlisted, createdAt, updatedAt);
    }

    private async Task<Template> ToDomainAsync(Models.Template row)
    {
        // Sanity checks.
        var applicableData = row.ApplicableData;

        if (applicableData == null)
        {
            throw new DataCorruptionException(row, "ApplicationData is null.");
        }

        var id = row.Id;

        // Load stats.
        var db = this.db.CreateMapper();
        var stats = await db.FirstOrDefaultAsync<Models.TemplateStats>("WHERE template_id = ?", id);
        long resumeCount;

        if (stats != null)
        {
            resumeCount = stats.ResumeCount;
        }
        else
        {
            resumeCount = 0;
        }

        return new(
            new(id),
            row.RegistrationId,
            applicableData,
            row.GetRenderOptions(),
            (RegistrationCategory)row.Category,
            row.ReleaseNote ?? string.Empty,
            resumeCount);
    }
}
