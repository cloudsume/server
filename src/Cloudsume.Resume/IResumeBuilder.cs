namespace Cloudsume.Resume
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Threading;
    using System.Threading.Tasks;
    using Candidate.Server.Resume;
    using Cloudsume.Services.Client;
    using Ultima.Extensions.Collections;

    public interface IResumeBuilder
    {
        /// <summary>
        /// Compile the specified template into LaTeX.
        /// </summary>
        /// <param name="template">
        /// StringTemplate to compile.
        /// </param>
        /// <param name="culture">
        /// Culture of the template.
        /// </param>
        /// <param name="options">
        /// Template options.
        /// </param>
        /// <param name="data">
        /// Data to compile.
        /// </param>
        /// <param name="job">
        /// Job for this build.
        /// </param>
        /// <param name="cancellationToken">
        /// The token to monitor for cancellation requests.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous build operation.
        /// </returns>
        /// <exception cref="TemplateSyntaxException">
        /// <paramref name="template"/> is not a valid StringTemplate.
        /// </exception>
        /// <exception cref="TemplateNotFoundException">
        /// <paramref name="template"/> does not contains resume template.
        /// </exception>
        Task<string> BuildAsync(
            string template,
            CultureInfo culture,
            IKeyedByTypeCollection<TemplateRenderOptions> options,
            IEnumerable<ResumeData> data,
            ILatexJobPoster job,
            CancellationToken cancellationToken = default);
    }
}
