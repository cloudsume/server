namespace Cloudsume.Resume
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Threading;
    using System.Threading.Tasks;
    using Candidate.Server.Resume;
    using Ultima.Extensions.Collections;

    public interface IResumeCompiler
    {
        /// <summary>
        /// Compile template assets and resume data to produce a PDF.
        /// </summary>
        /// <param name="culture">
        /// Template culture.
        /// </param>
        /// <param name="assets">
        /// Template assets to compile.
        /// </param>
        /// <param name="options">
        /// Options to compile main.stg.
        /// </param>
        /// <param name="data">
        /// Data to compile.
        /// </param>
        /// <param name="cancellationToken">
        /// The token to monitor for cancellation requests.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous compile operation.
        /// </returns>
        /// <exception cref="TemplateSyntaxException">
        /// main.stg is not a valid StringTemplate.
        /// </exception>
        /// <exception cref="TemplateNotFoundException">
        /// main.stg does not contains resume template.
        /// </exception>
        /// <exception cref="CompileException">
        /// LaTeX compilation error.
        /// </excepion>
        Task<CompileResult> CompileAsync(
            CultureInfo culture,
            IAsyncEnumerable<AssetFile> assets,
            IKeyedByTypeCollection<TemplateRenderOptions> options,
            IEnumerable<ResumeData> data,
            CancellationToken cancellationToken = default);
    }
}
