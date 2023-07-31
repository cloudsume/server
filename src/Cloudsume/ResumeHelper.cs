namespace Cloudsume
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Candidate.Server.Resume;
    using Cloudsume.Resume;
    using Cornot;

    internal sealed class ResumeHelper : IResumeHelper
    {
        private readonly IResumeRepository resumes;
        private readonly ITemplateRepository templates;
        private readonly ITemplateAssetRepository assets;
        private readonly IDataAggregator aggregator;
        private readonly IResumeCompiler compiler;
        private readonly IThumbnailGenerator thumbnailGenerator;
        private readonly IThumbnailRepository thumbnails;

        public ResumeHelper(
            IResumeRepository resumes,
            ITemplateRepository templates,
            ITemplateAssetRepository assets,
            IDataAggregator aggregator,
            IResumeCompiler compiler,
            IThumbnailGenerator thumbnailGenerator,
            IThumbnailRepository thumbnails)
        {
            this.resumes = resumes;
            this.templates = templates;
            this.assets = assets;
            this.aggregator = aggregator;
            this.compiler = compiler;
            this.thumbnailGenerator = thumbnailGenerator;
            this.thumbnails = thumbnails;
        }

        public async Task<CompileResult> CompileAsync(
            Candidate.Server.Resume.Resume resume,
            Cloudsume.Resume.Template? template = null,
            CancellationToken cancellationToken = default)
        {
            // Load template metadata.
            if (template == null)
            {
                template = await this.templates.GetTemplateAsync(resume.TemplateId, cancellationToken);

                if (template == null)
                {
                    throw new DataCorruptionException(resume, "Unknow template.");
                }
            }

            var culture = await this.templates.GetRegistrationCultureAsync(template.RegistrationId, cancellationToken);

            if (culture == null)
            {
                throw new DataCorruptionException(template, "Unknow registration.");
            }

            // Load global data.
            var globals = await ParentCollection.LoadAsync(culture, async culture =>
            {
                var data = await this.resumes.ListDataAsync(resume.UserId, culture, cancellationToken);

                return data.Select(d => d.Data);
            });

            // Create data set.
            var applicable = new HashSet<string>(template.ApplicableData.Select(t => t.ToString()));
            var data = this.aggregator.Aggregate(resume.Data.Where(d => applicable.Contains(d.Type)), globals);
            var assets = this.assets.ReadAsync(template.Id, cancellationToken);

            return await this.compiler.CompileAsync(culture, assets, template.RenderOptions, data, cancellationToken);
        }

        public async Task<int> UpdateThumbnailsAsync(
            Candidate.Server.Resume.Resume resume,
            Cloudsume.Resume.Template? template = null,
            CancellationToken cancellationToken = default)
        {
            await using var compile = await this.CompileAsync(resume, template, cancellationToken);
            var thumbnails = this.thumbnailGenerator.GenerateAsync(compile.PDF, cancellationToken);

            return await this.thumbnails.UpdateAsync(resume, thumbnails, cancellationToken);
        }
    }
}
