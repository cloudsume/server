namespace Cloudsume.Resume
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Candidate.Server.Resume;
    using Cloudsume.Services.Client;
    using Ultima.Extensions.Collections;

    public sealed class ResumeCompiler : IResumeCompiler
    {
        private readonly IResumeBuilder builder;
        private readonly IServicesClient services;

        public ResumeCompiler(IResumeBuilder builder, IServicesClient services)
        {
            this.builder = builder;
            this.services = services;
        }

        public async Task<CompileResult> CompileAsync(
            CultureInfo culture,
            IAsyncEnumerable<AssetFile> assets,
            IKeyedByTypeCollection<TemplateRenderOptions> options,
            IEnumerable<ResumeData> data,
            CancellationToken cancellationToken = default)
        {
            await using var job = this.services.NewLatexJob();

            // Upload assets.
            var uploader = new Uploader(job, this.builder, culture, options, data);

            await foreach (var asset in assets.WithCancellation(cancellationToken))
            {
                try
                {
                    await uploader.UploadAsync(asset, cancellationToken);
                }
                finally
                {
                    await asset.DisposeAsync();
                }
            }

            var source = uploader.Source;

            if (source == null)
            {
                throw new ArgumentException("No required item 'main.stg'.", nameof(assets));
            }

            // Compile.
            Stream pdf;

            try
            {
                pdf = await job.FinishAsync(cancellationToken);
            }
            catch (LatexJobException ex)
            {
                throw new CompileException(ex.Message, source);
            }

            try
            {
                return new CompileResult(source, pdf);
            }
            catch
            {
                await pdf.DisposeAsync();
                throw;
            }
        }

        private sealed class Uploader
        {
            private readonly ILatexJobPoster job;
            private readonly IResumeBuilder builder;
            private readonly CultureInfo culture;
            private readonly IKeyedByTypeCollection<TemplateRenderOptions> options;
            private readonly IEnumerable<ResumeData> data;

            public Uploader(
                ILatexJobPoster job,
                IResumeBuilder builder,
                CultureInfo culture,
                IKeyedByTypeCollection<TemplateRenderOptions> options,
                IEnumerable<ResumeData> data)
            {
                this.job = job;
                this.builder = builder;
                this.culture = culture;
                this.options = options;
                this.data = data;
            }

            public string? Source { get; set; }

            public async Task UploadAsync(AssetFile asset, CancellationToken cancellationToken = default)
            {
                if (asset.Name == "main.stg")
                {
                    // Load content.
                    string content;

                    using (var reader = new StreamReader(asset.Content, leaveOpen: true))
                    {
                        content = await reader.ReadToEndAsync();
                    }

                    // Compile.
                    this.Source = await this.builder.BuildAsync(content, this.culture, this.options, this.data, this.job, cancellationToken);

                    // Upload.
                    using var utf8 = new MemoryStream(Encoding.UTF8.GetBytes(this.Source));

                    await this.job.WriteAssetAsync("main.tex", (int)utf8.Length, utf8, cancellationToken);
                }
                else
                {
                    await this.job.WriteAssetAsync(asset.Name.Value, asset.Size, asset.Content, cancellationToken);
                }
            }
        }
    }
}
