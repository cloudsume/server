namespace Cloudsume.Aws
{
    using System;
    using Cloudsume.Resume;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    internal sealed class WorkspacePreviewRepository : ThumbnailRepository<Guid>, IWorkspacePreviewRepository
    {
        public WorkspacePreviewRepository(IOptions<WorkspacePreviewRepositoryOptions> options, ILogger<S3Repository> logger)
            : base(options, logger)
        {
        }

        protected override string GetDirectory(Guid id) => id.ToString();
    }
}
