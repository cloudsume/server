namespace Microsoft.Extensions.DependencyInjection
{
    using Candidate.Server.Resume;
    using Candidate.Server.Resume.Data.Repositories;
    using Cloudsume.Resume;
    using Cloudsume.Resume.Mergers;
    using Microsoft.Extensions.Configuration;

    public static class IServiceCollectionExtensions
    {
        public static void AddResume(this IServiceCollection services)
        {
            // Public services.
            services.AddSingleton(typeof(IDataActionCollection<>), typeof(DataActionCollection<>));
            services.AddSingleton<IDataAggregator, DataAggregator>();
            services.AddSingleton<ILinkDataCensor, LinkDataCensor>();
            services.AddSingleton<IResumeCompiler, ResumeCompiler>();
            services.AddSingleton<ISampleDataLoader, SampleDataLoader>();
            services.AddSingleton<IThumbnailGenerator, ThumbnailGenerator>();

            // Internal services.
            services.AddSingleton<IDataMerger, AddressMerger>();
            services.AddSingleton<IDataMerger, CertificateMerger>();
            services.AddSingleton<IDataMerger, EducationMerger>();
            services.AddSingleton<IDataMerger, EmailAddressMerger>();
            services.AddSingleton<IDataMerger, ExperienceMerger>();
            services.AddSingleton<IDataMerger, GitHubMerger>();
            services.AddSingleton<IDataMerger, HeadlineMerger>();
            services.AddSingleton<IDataMerger, LanguageMerger>();
            services.AddSingleton<IDataMerger, LinkedInMerger>();
            services.AddSingleton<IDataMerger, MobileMerger>();
            services.AddSingleton<IDataMerger, NameMerger>();
            services.AddSingleton<IDataMerger, PhotoMerger>();
            services.AddSingleton<IDataMerger, SkillMerger>();
            services.AddSingleton<IDataMerger, SummaryMerger>();
            services.AddSingleton<IDataMerger, WebsiteMerger>();
        }

        public static void AddFileSystemResumeTemplateAssetRepository(this IServiceCollection services, IConfiguration options)
        {
            services.AddOptions<FileSystemTemplateAssetRepositoryOptions>().Bind(options).ValidateDataAnnotations();
            services.AddSingleton<ITemplateAssetRepository, FileSystemTemplateAssetRepository>();
        }

        public static void AddFileSystemResumeThumbnailRepository(this IServiceCollection services, IConfiguration options)
        {
            services.AddOptions<FileSystemThumbnailRepositoryOptions>().Bind(options).ValidateDataAnnotations();
            services.AddSingleton<IThumbnailRepository, FileSystemThumbnailRepository>();
        }

        public static void AddFileSystemResumePhotoRepository(this IServiceCollection services, IConfiguration options)
        {
            services.AddOptions<PhotoImageRepositoryOptions>().Bind(options).ValidateDataAnnotations();
            services.AddSingleton<IPhotoImageRepository, PhotoImageRepository>();
        }

        public static void AddFileSystemSamplePhotoRepository(this IServiceCollection services, IConfiguration options)
        {
            services.AddOptions<FileSystemSamplePhotoRepositoryOptions>().Bind(options);
            services.AddSingleton<ISamplePhotoRepository, FileSystemSamplePhotoRepository>();
        }

        public static void AddFileSystemTemplatePreviewRepository(this IServiceCollection services, IConfiguration options)
        {
            services.AddOptions<FileSystemPreviewRepositoryOptions>().Bind(options).ValidateDataAnnotations();
            services.AddSingleton<ITemplatePreviewRepository, FileSystemPreviewRepository>();
        }

        public static void AddFileSystemWorkspaceAssetRepository(this IServiceCollection services, IConfiguration options)
        {
            services.AddOptions<FileSystemWorkspaceAssetRepositoryOptions>().Bind(options).ValidateDataAnnotations();
            services.AddSingleton<IWorkspaceAssetRepository, FileSystemWorkspaceAssetRepository>();
        }

        public static void AddFileSystemWorkspacePreviewRepository(this IServiceCollection services, IConfiguration options)
        {
            services.AddOptions<FileSystemWorkspacePreviewRepositoryOptions>().Bind(options).ValidateDataAnnotations();
            services.AddSingleton<IWorkspacePreviewRepository, FileSystemWorkspacePreviewRepository>();
        }
    }
}
