namespace Cloudsume.Resume
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Candidate.Server.Resume.Data;

    public interface ITemplateWorkspaceRepository
    {
        Task CreateAsync(Guid registrationId, TemplateWorkspace workspace, CancellationToken cancellationToken = default);

        Task<TemplateWorkspace?> GetAsync(Guid registrationId, CancellationToken cancellationToken = default);

        Task UpdatePreviewJobAsync(Guid registrationId, Guid? previewJob, CancellationToken cancellationToken = default);

        Task UpdateApplicableDataAsync(Guid registrationId, IEnumerable<string> data, CancellationToken cancellationToken = default);

        Task UpdateRenderOptionsAsync(Guid registrationId, EducationRenderOptions? options, CancellationToken cancellationToken = default);

        Task UpdateRenderOptionsAsync(Guid registrationId, ExperienceRenderOptions? options, CancellationToken cancellationToken = default);

        Task UpdateRenderOptionsAsync(Guid registrationId, SkillRenderOptions? options, CancellationToken cancellationToken = default);

        /// <summary>
        /// Update or add asset metadata in the specified workspace.
        /// </summary>
        /// <param name="registrationId">
        /// Workspace to update.
        /// </param>
        /// <param name="asset">
        /// Asset to update.
        /// </param>
        /// <param name="cancellationToken">
        /// The token to monitor for cancellation requests.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous update operation.
        /// </returns>
        Task UpdateAssetAsync(Guid registrationId, TemplateAsset asset, CancellationToken cancellationToken = default);

        Task DeleteAssetAsync(Guid registrationId, AssetName name, CancellationToken cancellationToken = default);
    }
}
