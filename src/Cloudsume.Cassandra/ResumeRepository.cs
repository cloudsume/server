namespace Cloudsume.Cassandra;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Candidate.Server.Resume;
using Candidate.Server.Resume.Data;
using Cloudsume.Resume;
using Cloudsume.Server.Cassandra;
using Cornot;
using global::Cassandra;
using global::Cassandra.Mapping;
using NetUlid;

internal sealed class ResumeRepository : IResumeRepository
{
    private readonly IMapperFactory db;
    private readonly IReadConsistencyProvider readConsistencies;
    private readonly IDataActionCollection<IResumeDataMapper> dataMappers;
    private readonly IDataActionCollection<IResumeDataPayloadManager> dataPayloads;
    private readonly IReadOnlyDictionary<string, ResumeDataManager> dataManagers;

    public ResumeRepository(
        IMapperFactory db,
        IReadConsistencyProvider readConsistencies,
        IDataActionCollection<IResumeDataMapper> dataMappers,
        IDataActionCollection<IResumeDataPayloadManager> dataPayloads)
    {
        this.db = db;
        this.readConsistencies = readConsistencies;
        this.dataMappers = dataMappers;
        this.dataPayloads = dataPayloads;
        this.dataManagers = ResumeDataManager.BuildTable(db.CreateMapper().GetType());
    }

    public async Task<int> CountAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var db = this.db.CreateMapper();

        return (await db.FetchAsync<Models.Resume>("WHERE user_id = ?", userId)).Count();
    }

    public async Task CreateAsync(Resume resume, CancellationToken cancellationToken = default)
    {
        var db = this.db.CreateMapper();

        // Insert resume.
        var row = new Models.Resume()
        {
            UserId = resume.UserId,
            Id = resume.Id,
            Name = resume.Name,
            TemplateId = resume.TemplateId.ToByteArray(),
            CreatedAt = resume.CreatedAt,
        };

        if (resume.RecruitmentConsent)
        {
            row.RecruitmentConsent = DateTime.UtcNow.ToLocalDate();
        }

        await db.InsertAsync(row, CqlQueryOptions.New().SetConsistencyLevel(ConsistencyLevel.EachQuorum));

        // Insert data.
        foreach (var group in resume.Data.GroupBy(d => d.Type))
        {
            var manager = this.dataManagers[group.Key];
            var mapper = this.dataMappers[group.Key];
            var domains = group.ToArray();

            this.dataPayloads.TryGetValue(group.Key, out var payload);

            for (var i = 0; i < domains.Length; i++)
            {
                var domain = domains[i];
                var dto = manager.NewDto();

                dto.UserId = resume.UserId;
                dto.ResumeId = resume.Id;
                dto.Language = string.Empty;
                dto.Data = mapper.ToCassandra(domain);

                if (dto is IMultiplicativeResumeData m)
                {
                    if (((MultiplicativeData)domain).Id != Guid.Empty)
                    {
                        throw new ArgumentException($"Some data has non-empty {nameof(MultiplicativeData.Id)}.", nameof(resume));
                    }

                    m.Index = Convert.ToSByte(i);
                    m.Id = Guid.Empty;
                    m.BaseId = ((MultiplicativeData)domain).BaseId;

                    if (payload != null)
                    {
                        await payload.UpdatePayloadAsync(domain, dto, i);
                    }
                }
                else if (payload != null)
                {
                    await payload.UpdatePayloadAsync(domain, dto, null);
                }

                await manager.UpdateAsync(db, dto, CqlQueryOptions.New().SetConsistencyLevel(ConsistencyLevel.LocalQuorum));
            }
        }
    }

    public async Task DeleteAsync(Guid userId, Guid id, CancellationToken cancellationToken = default)
    {
        var db = this.db.CreateMapper();

        // Revoke recruitment consent.
        var target = await db.FirstOrDefaultAsync<Models.Resume>(Cql.New("WHERE user_id = ? AND id = ?", userId, id).WithOptions(options =>
        {
            options.SetConsistencyLevel(this.readConsistencies.StrongConsistency);
        }));

        if (target is null)
        {
            return;
        }
        else if (target.RecruitmentConsent is not null)
        {
            var revoke = new Models.RecruitmentRevoke()
            {
                Date = DateTime.UtcNow.ToLocalDate(),
                UserId = userId,
                ResumeId = id,
            };

            await db.InsertAsync(revoke);
        }

        // Delete resume.
        var delete = Cql.New("WHERE user_id = ? AND id = ?", userId, id).WithOptions(options =>
        {
            options.SetConsistencyLevel(ConsistencyLevel.EachQuorum);
        });

        await db.DeleteAsync<Models.Resume>(delete);

        // Delete data.
        foreach (var (type, manager) in this.dataManagers)
        {
            await manager.ClearAsync(db, userId, id);

            if (this.dataPayloads.TryGetValue(type, out var payload))
            {
                await payload.ClearPayloadAsync(userId, id);
            }
        }
    }

    public async Task DeleteDataAsync(Resume resume, IEnumerable<LocalDataDelete> data, CancellationToken cancellationToken = default)
    {
        var db = this.db.CreateMapper();

        foreach (var delete in data)
        {
            var manager = this.dataManagers[delete.Type];
            var target = manager.NewDto();

            target.UserId = resume.UserId;
            target.ResumeId = resume.Id;
            target.Language = string.Empty;

            if (target is IMultiplicativeResumeData m)
            {
                var index = delete.Index;

                if (index == null)
                {
                    throw new ArgumentException($"Deleting '{delete.Type}' required to specify {nameof(delete.Index)}.", nameof(data));
                }

                m.Index = Convert.ToSByte(index.Value);
                m.Id = Guid.Empty;
            }
            else if (delete.Index != null)
            {
                throw new ArgumentException($"Deleting '{delete.Type}' required {nameof(delete.Index)} to be null.", nameof(data));
            }

            await manager.DeleteAsync(db, target, CqlQueryOptions.New().SetConsistencyLevel(ConsistencyLevel.LocalQuorum));

            // Delete payload.
            if (this.dataPayloads.TryGetValue(delete.Type, out var payload))
            {
                await payload.DeletePayloadAsync(resume.UserId, resume.Id, delete.Index);
            }
        }
    }

    public async Task<Resume?> GetAsync(Guid userId, Guid id, CancellationToken cancellationToken = default)
    {
        var db = this.db.CreateMapper();
        var query = Cql.New("WHERE user_id = ? AND id = ?", userId, id).WithOptions(options =>
        {
            options.SetConsistencyLevel(this.readConsistencies.StrongConsistency);
        });

        var row = await db.FirstOrDefaultAsync<Models.Resume>(query);

        if (row == null)
        {
            return null;
        }

        return await this.ToDomainAsync(row, cancellationToken);
    }

    public async Task<Photo?> GetPhotoAsync(Guid userId, Guid resumeId, CancellationToken cancellationToken = default)
    {
        var db = this.db.CreateMapper();

        // Fetch data.
        var manager = this.dataManagers[Photo.StaticType];
        var row = (Models.ResumePhoto?)await manager.GetAsync(db, userId, resumeId, this.readConsistencies.StrongConsistency);

        if (row == null)
        {
            return null;
        }

        var data = row.Data;

        if (data == null)
        {
            throw new DataCorruptionException(row, $"{nameof(row.Data)} is null.");
        }

        // Convert to domain object.
        var mapper = this.dataMappers[Photo.StaticType];
        var domain = mapper.ToDomain(Guid.Empty, null, data);

        // Load payload.
        var payload = this.dataPayloads[Photo.StaticType];

        await payload.ReadPayloadAsync(domain, row, null, cancellationToken);

        return (Photo)domain;
    }

    public async Task<IEnumerable<Resume>> ListAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var db = this.db.CreateMapper();
        var query = Cql.New("WHERE user_id = ?", userId).WithOptions(options => options.SetConsistencyLevel(this.readConsistencies.StrongConsistency));
        var rows = await db.FetchAsync<Models.Resume>(query);
        var domains = new List<Resume>();

        foreach (var row in rows)
        {
            domains.Add(await this.ToDomainAsync(row, cancellationToken));
        }

        return domains;
    }

    public async Task<IEnumerable<GlobalData>> ListDataAsync(Guid userId, CultureInfo? culture, CancellationToken cancellationToken = default)
    {
        var db = this.db.CreateMapper();
        var result = new List<GlobalData>();

        foreach (var (type, manager) in this.dataManagers)
        {
            var mapper = this.dataMappers[type];
            Cql query;

            if (culture != null)
            {
                query = Cql.New("WHERE user_id = ? AND resume_id = ? AND language = ?", userId, Guid.Empty, culture.Name);
            }
            else
            {
                query = Cql.New("WHERE user_id = ? AND resume_id = ?", userId, Guid.Empty);
            }

            query.WithOptions(options => options.SetConsistencyLevel(this.readConsistencies.StrongConsistency));

            this.dataPayloads.TryGetValue(type, out var payload);

            foreach (var row in await manager.GetAsync(db, query))
            {
                var data = row.Data;

                if (data == null)
                {
                    throw new DataCorruptionException(row, $"{nameof(row.Data)} is null.");
                }

                // Create domain object.
                ResumeData domain;

                if (row is IMultiplicativeResumeData m)
                {
                    domain = mapper.ToDomain(m.Id, m.BaseId, data);
                }
                else
                {
                    domain = mapper.ToDomain(Guid.Empty, null, data);
                }

                // Get payload.
                if (payload != null)
                {
                    await payload.ReadPayloadAsync(domain, row, null, cancellationToken);
                }

                result.Add(mapper.CreateGlobal(CultureInfo.GetCultureInfo(row.Language), domain));
            }
        }

        return result;
    }

    public async Task<IEnumerable<ResumeData>> ListDataAsync(Guid userId, Guid id, CancellationToken cancellationToken = default)
    {
        var db = this.db.CreateMapper();
        var results = new List<ResumeData>();

        foreach (var (type, manager) in this.dataManagers)
        {
            var mapper = this.dataMappers[type];
            var result = await manager.GetAsync(db, userId, id, this.readConsistencies.StrongConsistency);

            this.dataPayloads.TryGetValue(type, out var payload);

            if (result is IEnumerable<IMultiplicativeResumeData> rows)
            {
                var i = 0;

                foreach (var row in rows)
                {
                    var data = row.Data;

                    if (data == null)
                    {
                        throw new DataCorruptionException(row, $"{nameof(row.Data)} is null.");
                    }

                    var domain = mapper.ToDomain(row.Id, row.BaseId, data);

                    if (payload != null)
                    {
                        await payload.ReadPayloadAsync(domain, row, i, cancellationToken);
                    }

                    results.Add(domain);
                    i++;
                }
            }
            else if (result is IResumeData row)
            {
                var data = row.Data;

                if (data == null)
                {
                    throw new DataCorruptionException(row, $"{nameof(row.Data)} is null.");
                }

                var domain = mapper.ToDomain(Guid.Empty, null, data);

                if (payload != null)
                {
                    await payload.ReadPayloadAsync(domain, row, null, cancellationToken);
                }

                results.Add(domain);
            }
            else if (result != null)
            {
                throw new NotImplementedException($"Row {result.GetType()} is not implemented.");
            }
        }

        return results;
    }

    public async Task<IEnumerable<GlobalData<Photo>>> ListPhotoAsync(Guid userId, CultureInfo? culture, CancellationToken cancellationToken = default)
    {
        var db = this.db.CreateMapper();
        var results = new List<GlobalData<Photo>>();
        var manager = this.dataManagers[Photo.StaticType];
        var mapper = this.dataMappers[Photo.StaticType];
        var payload = this.dataPayloads[Photo.StaticType];
        Cql query;

        if (culture != null)
        {
            query = Cql.New("WHERE user_id = ? AND resume_id = ? AND language = ?", userId, Guid.Empty, culture.Name);
        }
        else
        {
            query = Cql.New("WHERE user_id = ? AND resume_id = ?", userId, Guid.Empty);
        }

        query.WithOptions(options => options.SetConsistencyLevel(this.readConsistencies.StrongConsistency));

        foreach (var row in await manager.GetAsync(db, query))
        {
            var data = row.Data;

            if (data == null)
            {
                throw new DataCorruptionException(row, $"{nameof(row.Data)} is null.");
            }

            var domain = mapper.ToDomain(Guid.Empty, null, data);

            await payload.ReadPayloadAsync(domain, row, null, cancellationToken);

            results.Add(new(CultureInfo.GetCultureInfo(row.Language), (Photo)domain));
        }

        return results;
    }

    public async Task SetRecruitmentConsentAsync(ResumeInfo resume, bool value, CancellationToken cancellationToken = default)
    {
        var db = this.db.CreateMapper();
        var userId = resume.UserId;
        var id = resume.Id;

        if (value)
        {
            // Do nothing if already consent.
            if (resume.RecruitmentConsent)
            {
                return;
            }

            // Make sure no revocation.
            var date = DateTime.UtcNow.ToLocalDate();

            await db.DeleteAsync<Models.RecruitmentRevoke>("WHERE date = ? AND user = ? AND resume = ?", date, userId, id);

            // Set consent.
            await db.UpdateAsync<Models.Resume>(Cql.New("SET recruitment_consent = ? WHERE user_id = ? AND id = ?", date, userId, id).WithOptions(options =>
            {
                options.SetConsistencyLevel(ConsistencyLevel.EachQuorum);
            }));
        }
        else if (resume.RecruitmentConsent)
        {
            // Remove consent.
            await db.UpdateAsync<Models.Resume>(Cql.New("SET recruitment_consent = null WHERE user_id = ? AND id = ?", userId, id).WithOptions(options =>
            {
                options.SetConsistencyLevel(ConsistencyLevel.EachQuorum);
            }));

            // Revoke.
            var revoke = new Models.RecruitmentRevoke()
            {
                Date = DateTime.UtcNow.ToLocalDate(),
                UserId = userId,
                ResumeId = id,
            };

            await db.InsertAsync(revoke);
        }
    }

    public Task SetUpdatedAsync(ResumeInfo resume, DateTime time, CancellationToken cancellationToken = default)
    {
        var db = this.db.CreateMapper();

        // Build the query.
        var query = new StringBuilder("SET updated_at = ?");
        var args = new List<object>()
        {
            new DateTimeOffset(time),
        };

        if (resume.RecruitmentConsent)
        {
            query.Append(", recruitment_consent = ?");
            args.Add(DateTime.UtcNow.ToLocalDate());
        }

        query.Append(" WHERE user_id = ? AND id = ?");
        args.Add(resume.UserId);
        args.Add(resume.Id);

        // Update.
        return db.UpdateAsync<Models.Resume>(Cql.New(query.ToString(), args.ToArray()).WithOptions(options =>
        {
            options.SetConsistencyLevel(ConsistencyLevel.EachQuorum);
        }));
    }

    public async Task<IEnumerable<Guid>> TransferAsync(Guid from, Guid to, CancellationToken cancellationToken = default)
    {
        var db = this.db.CreateMapper();
        var result = new List<Guid>();
        var query = Cql.New("WHERE user_id = ?", from).WithOptions(options =>
        {
            options.SetConsistencyLevel(this.readConsistencies.StrongConsistency);
        });

        foreach (var resume in await db.FetchAsync<Models.Resume>(query))
        {
            // Update recruitment consent.
            if (resume.RecruitmentConsent is not null)
            {
                var revoke = new Models.RecruitmentRevoke()
                {
                    Date = DateTime.UtcNow.ToLocalDate(),
                    UserId = from,
                    ResumeId = resume.Id,
                };

                await db.InsertAsync(revoke);

                resume.RecruitmentConsent = DateTime.UtcNow.ToLocalDate();
            }

            // Create a copy on target user.
            resume.UserId = to;

            await db.InsertAsync(resume, CqlQueryOptions.New().SetConsistencyLevel(ConsistencyLevel.EachQuorum));

            // Delete from source user.
            resume.UserId = from;

            await db.DeleteAsync(resume);

            result.Add(resume.Id);
        }

        return result;
    }

    public async Task<bool> TransferDataAsync(Guid from, Guid to, CancellationToken cancellationToken = default)
    {
        var db = this.db.CreateMapper();

        // Check if destination user already have some data.
        foreach (var (_, manager) in this.dataManagers)
        {
            var rows = await manager.GetAsync(db, Cql.New("WHERE user_id = ?", to));

            if (rows.Any())
            {
                return false;
            }
        }

        // Move data.
        foreach (var (type, manager) in this.dataManagers)
        {
            var query = Cql.New("WHERE user_id = ?", from).WithOptions(options =>
            {
                // The ideal consistency here is each quorum but Cassandra does not support it.
                options.SetConsistencyLevel(this.readConsistencies.StrongConsistency);
            });

            foreach (var source in await manager.GetAsync(db, query))
            {
                // Move data.
                var copy = manager.NewDto();

                copy.UserId = to;
                copy.ResumeId = source.ResumeId;
                copy.Language = source.Language;
                copy.Data = source.Data;

                if (copy is IMultiplicativeResumeData m)
                {
                    var s = (IMultiplicativeResumeData)source;

                    m.Index = s.Index;
                    m.Id = s.Id;
                    m.BaseId = s.BaseId;
                }

                await manager.UpdateAsync(db, copy, CqlQueryOptions.New().SetConsistencyLevel(ConsistencyLevel.LocalQuorum));
                await manager.DeleteAsync(db, source, CqlQueryOptions.New());

                // Move payload.
                if (this.dataPayloads.TryGetValue(type, out var payload))
                {
                    await payload.TransferPayloadAsync(source, to);
                }
            }
        }

        return true;
    }

    public async Task<IEnumerable<ResumeData>> UpdateDataAsync(
        Resume resume,
        IEnumerable<LocalDataUpdate> updates,
        CancellationToken cancellationToken = default)
    {
        var db = this.db.CreateMapper();

        // Update data.
        foreach (var update in updates)
        {
            var domain = update.Value;
            var manager = this.dataManagers[domain.Type];
            var mapper = this.dataMappers[domain.Type];

            // Update row.
            var dto = manager.NewDto();
            int? index;

            dto.UserId = resume.UserId;
            dto.ResumeId = resume.Id;
            dto.Language = string.Empty;
            dto.Data = mapper.ToCassandra(domain);

            if (dto is IMultiplicativeResumeData m)
            {
                index = ((MultiplicativeLocalDataUpdate)update).Index;

                if (((MultiplicativeData)domain).Id != Guid.Empty)
                {
                    throw new ArgumentException($"Some updates has non-empty {nameof(MultiplicativeData.Id)}.", nameof(updates));
                }

                m.Index = Convert.ToSByte(index.Value);
                m.Id = Guid.Empty;
                m.BaseId = ((MultiplicativeData)domain).BaseId;
            }
            else
            {
                index = null;
            }

            await manager.UpdateAsync(db, dto, CqlQueryOptions.New().SetConsistencyLevel(ConsistencyLevel.LocalQuorum));

            // Write payload.
            if (this.dataPayloads.TryGetValue(domain.Type, out var payload))
            {
                await payload.UpdatePayloadAsync(domain, dto, index);
            }
        }

        return this.MergeData(
            resume.Data.GroupBy(d => d.Type).ToDictionary(g => g.Key, g => g.ToArray()),
            updates.GroupBy(u => u.Value.Type).ToDictionary(g => g.Key, g => g.ToArray()));
    }

    public async Task UpdateDataAsync(Guid userId, CultureInfo culture, IEnumerable<ResumeData> data, CancellationToken cancellationToken = default)
    {
        var db = this.db.CreateMapper();

        foreach (var entry in data)
        {
            var manager = this.dataManagers[entry.Type];
            var mapper = this.dataMappers[entry.Type];

            // Update row.
            var dto = manager.NewDto();

            dto.UserId = userId;
            dto.ResumeId = Guid.Empty;
            dto.Language = culture.Name;
            dto.Data = mapper.ToCassandra(entry);

            if (dto is IMultiplicativeResumeData m)
            {
                var id = ((MultiplicativeData)entry).Id;

                if (id == null)
                {
                    throw new ArgumentException("Some item is an agregated data.", nameof(data));
                }
                else if (id == Guid.Empty)
                {
                    throw new ArgumentException($"Some item has empty {nameof(MultiplicativeData.Id)}.", nameof(data));
                }

                m.Index = 0;
                m.Id = id.Value;
                m.BaseId = ((MultiplicativeData)entry).BaseId;
            }

            await manager.UpdateAsync(db, dto, CqlQueryOptions.New().SetConsistencyLevel(ConsistencyLevel.LocalQuorum));

            // Write payload.
            if (this.dataPayloads.TryGetValue(entry.Type, out var payload))
            {
                await payload.UpdatePayloadAsync(entry, dto, null);
            }
        }
    }

    public async Task UpdateNameAsync(Guid userId, Guid id, string name, CancellationToken cancellationToken = default)
    {
        var db = this.db.CreateMapper();
        var query = Cql.New("SET name = ?, updated_at = ? WHERE user_id = ? AND id = ?", name, new DateTimeOffset(DateTime.Now), userId, id);

        await db.UpdateAsync<Models.Resume>(query.WithOptions(options => options.SetConsistencyLevel(this.readConsistencies.StrongConsistency)));
    }

    public async Task UpdateTemplateAsync(Guid userId, Guid id, Ulid templateId, CancellationToken cancellationToken = default)
    {
        var db = this.db.CreateMapper();
        var query = Cql.New(
            "SET template_id = ?, updated_at = ? WHERE user_id = ? AND id = ?",
            templateId.ToByteArray(),
            new DateTimeOffset(DateTime.Now),
            userId,
            id);

        await db.UpdateAsync<Models.Resume>(query.WithOptions(options => options.SetConsistencyLevel(ConsistencyLevel.LocalQuorum)));
    }

    private IEnumerable<ResumeData> MergeData(IReadOnlyDictionary<string, ResumeData[]> @base, IReadOnlyDictionary<string, LocalDataUpdate[]> source)
    {
        var result = new List<ResumeData>();

        foreach (var (type, _) in this.dataManagers)
        {
            // Use the update value if available.
            if (source.TryGetValue(type, out var updates))
            {
                if (updates[0] is MultiplicativeLocalDataUpdate)
                {
                    var values = new SortedDictionary<int, ResumeData>();

                    // Fill with base value then override by the updates.
                    if (@base.TryGetValue(type, out var origin))
                    {
                        for (var i = 0; i < origin.Length; i++)
                        {
                            values.Add(i, origin[i]);
                        }
                    }

                    foreach (MultiplicativeLocalDataUpdate u in updates)
                    {
                        values[u.Index] = u.Value;
                    }

                    result.AddRange(values.Values);
                }
                else
                {
                    result.Add(updates.Single().Value);
                }
            }
            else if (@base.TryGetValue(type, out var values))
            {
                result.AddRange(values);
            }
        }

        return result;
    }

    private async Task<Resume> ToDomainAsync(Models.Resume row, CancellationToken cancellationToken = default)
    {
        if (row.Name is not { } name)
        {
            throw new DataCorruptionException(row, "Name is null.");
        }

        if (row.TemplateId is not { } template)
        {
            throw new DataCorruptionException(row, "Template identifier is null.");
        }

        var data = await this.ListDataAsync(row.UserId, row.Id, cancellationToken);

        return new(
            row.UserId,
            row.Id,
            name,
            new(template),
            row.RecruitmentConsent is not null,
            row.CreatedAt.LocalDateTime,
            row.UpdatedAt?.LocalDateTime,
            data);
    }
}
