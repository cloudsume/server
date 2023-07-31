namespace Microsoft.Extensions.DependencyInjection
{
    using Candidate.Server.Aws;
    using Candidate.Server.Resume;
    using Candidate.Server.Resume.Data.Repositories;
    using Cloudsume.Analytics;
    using Cloudsume.Aws;
    using Cloudsume.Resume;
    using Microsoft.Extensions.Configuration;

    public static class IServiceCollectionExtensions
    {
        public static void AddAwsSamplePhotoRepository(this IServiceCollection services, IConfiguration options)
        {
            services.AddOptions<SamplePhotoRepositoryOptions>().Bind(options);
            services.AddSingleton<ISamplePhotoRepository, SamplePhotoRepository>();
        }

        public static void AddAwsResumePhotoRepository(this IServiceCollection services, IConfiguration options)
        {
            services.AddOptions<ResumePhotoRepositoryOptions>().Bind(options).ValidateDataAnnotations();
            services.AddSingleton<IPhotoImageRepository, ResumePhotoRepository>();
        }

        public static void AddAwsTemplateAssetRepository(this IServiceCollection services, IConfiguration options)
        {
            services.AddOptions<TemplateAssetRepositoryOptions>().Bind(options).ValidateDataAnnotations();
            services.AddSingleton<ITemplateAssetRepository, TemplateAssetRepository>();
        }

        public static void AddAwsResumeThumbnailRepository(this IServiceCollection services, IConfiguration options)
        {
            services.AddOptions<ResumeThumbnailRepositoryOptions>().Bind(options).ValidateDataAnnotations();
            services.AddSingleton<IThumbnailRepository, ResumeThumbnailRepository>();
        }

        public static void AddAwsStatsRepository(this IServiceCollection services, IConfiguration options)
        {
            services.AddOptions<StatsRepositoryOptions>().Bind(options).ValidateDataAnnotations();
            services.AddSingleton<IStatsRepository, StatsRepository>();
        }

        public static void AddAwsTemplatePreviewRepository(this IServiceCollection services, IConfiguration options)
        {
            services.AddOptions<TemplatePreviewRepositoryOptions>().Bind(options).ValidateDataAnnotations();
            services.AddSingleton<ITemplatePreviewRepository, TemplatePreviewRepository>();
        }

        public static void AddAwsWorkspaceAssetRepository(this IServiceCollection services, IConfiguration options)
        {
            services.AddOptions<WorkspaceAssetRepositoryOptions>().Bind(options).ValidateDataAnnotations();
            services.AddSingleton<IWorkspaceAssetRepository, WorkspaceAssetRepository>();
        }

        public static void AddAwsWorkspacePreviewRepository(this IServiceCollection services, IConfiguration options)
        {
            services.AddOptions<WorkspacePreviewRepositoryOptions>().Bind(options).ValidateDataAnnotations();
            services.AddSingleton<IWorkspacePreviewRepository, WorkspacePreviewRepository>();
        }
    }
}
