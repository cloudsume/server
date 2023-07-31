namespace Cloudsume.Cassandra
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Candidate.Server.Resume.Data;
    using Cloudsume.Resume;
    using Cornot;
    using global::Cassandra;
    using global::Cassandra.Mapping;

    public sealed class TemplateWorkspaceRepository : ITemplateWorkspaceRepository
    {
        private readonly IMapperFactory db;
        private readonly IReadConsistencyProvider readConsistencies;

        public TemplateWorkspaceRepository(IMapperFactory db, IReadConsistencyProvider readConsistencies)
        {
            this.db = db;
            this.readConsistencies = readConsistencies;
        }

        public async Task CreateAsync(Guid registrationId, TemplateWorkspace workspace, CancellationToken cancellationToken = default)
        {
            var db = this.db.CreateMapper();
            var row = new Models.TemplateWorkspace()
            {
                RegistrationId = registrationId,
                PreviewJob = workspace.PreviewJob,
                ApplicableData = workspace.ApplicableData,
                ExperienceOptions = workspace.RenderOptions.GetExperienceOptions(),
                EducationOptions = workspace.RenderOptions.GetEducationOptions(),
                SkillOptions = workspace.RenderOptions.GetSkillOptions(),
                Assets = workspace.Assets.ToDictionary(a => a.Name.Value, a => this.ToDto(a)),
            };

            await db.InsertAsync(row, CqlQueryOptions.New().SetConsistencyLevel(ConsistencyLevel.EachQuorum));
        }

        public async Task DeleteAssetAsync(Guid registrationId, AssetName name, CancellationToken cancellationToken = default)
        {
            var db = this.db.CreateMapper();
            var query = Cql.New("SET assets = assets - ? WHERE registration_id = ?", new[] { name.Value }, registrationId).WithOptions(options =>
            {
                options.SetConsistencyLevel(ConsistencyLevel.LocalQuorum);
            });

            await db.UpdateAsync<Models.TemplateWorkspace>(query);
        }

        public async Task<TemplateWorkspace?> GetAsync(Guid registrationId, CancellationToken cancellationToken = default)
        {
            var db = this.db.CreateMapper();
            var query = Cql.New("WHERE registration_id = ?", registrationId).WithOptions(options =>
            {
                options.SetConsistencyLevel(this.readConsistencies.StrongConsistency);
            });

            var row = await db.FirstOrDefaultAsync<Models.TemplateWorkspace>(query);

            if (row == null)
            {
                return null;
            }

            return this.ToDomain(row);
        }

        public async Task UpdateApplicableDataAsync(Guid registrationId, IEnumerable<string> data, CancellationToken cancellationToken = default)
        {
            var db = this.db.CreateMapper();
            var query = Cql.New("SET applicable_data = ? WHERE registration_id = ?", data, registrationId).WithOptions(options =>
            {
                options.SetConsistencyLevel(ConsistencyLevel.LocalQuorum);
            });

            await db.UpdateAsync<Models.TemplateWorkspace>(query);
        }

        public async Task UpdateAssetAsync(Guid registrationId, TemplateAsset asset, CancellationToken cancellationToken = default)
        {
            var db = this.db.CreateMapper();
            var column = new Dictionary<string, Models.TemplateAsset>()
            {
                { asset.Name.Value, this.ToDto(asset) },
            };

            var query = Cql.New("SET assets = assets + ? WHERE registration_id = ?", column, registrationId).WithOptions(options =>
            {
                options.SetConsistencyLevel(ConsistencyLevel.LocalQuorum);
            });

            await db.UpdateAsync<Models.TemplateWorkspace>(query);
        }

        public async Task UpdatePreviewJobAsync(Guid registrationId, Guid? previewJob, CancellationToken cancellationToken = default)
        {
            var db = this.db.CreateMapper();
            var query = Cql.New("SET preview_job = ? WHERE registration_id = ?", previewJob, registrationId).WithOptions(options =>
            {
                options.SetConsistencyLevel(ConsistencyLevel.LocalQuorum);
            });

            await db.UpdateAsync<Models.TemplateWorkspace>(query);
        }

        public async Task UpdateRenderOptionsAsync(Guid registrationId, EducationRenderOptions? options, CancellationToken cancellationToken = default)
        {
            var db = this.db.CreateMapper();
            var query = Cql.New("SET education_options = ? WHERE registration_id = ?", this.ToDto(options), registrationId).WithOptions(options =>
            {
                options.SetConsistencyLevel(ConsistencyLevel.LocalQuorum);
            });

            await db.UpdateAsync<Models.TemplateWorkspace>(query);
        }

        public async Task UpdateRenderOptionsAsync(Guid registrationId, ExperienceRenderOptions? options, CancellationToken cancellationToken = default)
        {
            var db = this.db.CreateMapper();
            var query = Cql.New("SET experience_options = ? WHERE registration_id = ?", this.ToDto(options), registrationId).WithOptions(options =>
            {
                options.SetConsistencyLevel(ConsistencyLevel.LocalQuorum);
            });

            await db.UpdateAsync<Models.TemplateWorkspace>(query);
        }

        public async Task UpdateRenderOptionsAsync(Guid registrationId, SkillRenderOptions? options, CancellationToken cancellationToken = default)
        {
            var db = this.db.CreateMapper();
            var query = Cql.New("SET skill_options = ? WHERE registration_id = ?", this.ToDto(options), registrationId).WithOptions(options =>
            {
                options.SetConsistencyLevel(ConsistencyLevel.LocalQuorum);
            });

            await db.UpdateAsync<Models.TemplateWorkspace>(query);
        }

        private Models.TemplateEducationOptions? ToDto(EducationRenderOptions? domain)
        {
            if (domain == null)
            {
                return null;
            }

            return new()
            {
                DescriptionParagraph = domain.DescriptionParagraph,
                DescriptionListOptions = domain.DescriptionListOptions,
            };
        }

        private Models.TemplateExperienceOptions? ToDto(ExperienceRenderOptions? domain)
        {
            if (domain == null)
            {
                return null;
            }

            return new()
            {
                DescriptionParagraph = domain.DescriptionParagraph,
                DescriptionListOptions = domain.DescriptionListOptions,
            };
        }

        private Models.TemplateSkillOptions? ToDto(SkillRenderOptions? domain)
        {
            if (domain == null)
            {
                return null;
            }

            return new()
            {
                Grouping = Convert.ToSByte(domain.Grouping),
            };
        }

        private Models.TemplateAsset ToDto(TemplateAsset domain) => new()
        {
            Name = domain.Name.Value,
            Size = domain.Size,
            LastModified = domain.LastModified,
        };

        private TemplateWorkspace ToDomain(Models.TemplateWorkspace row)
        {
            if (row.ApplicableData == null)
            {
                throw new DataCorruptionException(row, $"{nameof(row.ApplicableData)} is null.");
            }

            if (row.Assets == null)
            {
                throw new DataCorruptionException(row, $"{nameof(row.Assets)} is null.");
            }

            return new(
                row.PreviewJob,
                row.ApplicableData,
                row.GetRenderOptions(),
                row.Assets.Select(i => new TemplateAsset(new(i.Key), i.Value.Size, i.Value.LastModified.LocalDateTime)).ToHashSet());
        }
    }
}
