namespace Microsoft.Extensions.DependencyInjection
{
    using Candidate.Server.Resume;
    using Cloudsume.Analytics;
    using Cloudsume.Cassandra;
    using Cloudsume.Cassandra.ResumeDataMappers;
    using Cloudsume.Configurations;
    using Cloudsume.Data;
    using Cloudsume.Financial;
    using Cloudsume.Identity;
    using Cloudsume.Resume;
    using Cloudsume.Template;

    public static class IServiceCollectionExtensions
    {
        public static void AddCassandraRepositories(this IServiceCollection services)
        {
            // Public services.
            services.AddSingleton<ICancelPurchaseFeedbackRepository, TemplateCancelPurchaseFeedbackRepository>();
            services.AddSingleton<IConfigurationRepository, ConfigurationRepository>();
            services.AddSingleton<IFeedbackRepository, FeedbackRepository>();
            services.AddSingleton<IGuestSessionRepository, GuestSessionRepository>();
            services.AddSingleton<IJobRepository, JobRepository>();
            services.AddSingleton<ILinkAccessRepository, ResumeLinkAccessRepository>();
            services.AddSingleton<IReceivingMethodRepository, ReceivingMethodRepository>();
            services.AddSingleton<IResumeLinkRepository, ResumeLinkRepository>();
            services.AddSingleton<IResumeRepository, ResumeRepository>();
            services.AddSingleton<ISampleDataRepository, SampleDataRepository>();
            services.AddSingleton<ITemplateLicenseRepository, TemplateLicenseRepository>();
            services.AddSingleton<ITemplatePackLicenseRepository, TemplatePackLicenseRepository>();
            services.AddSingleton<ITemplatePackRepository, TemplatePackRepository>();
            services.AddSingleton<ITemplateRepository, TemplateRepository>();
            services.AddSingleton<ITemplateWorkspaceRepository, TemplateWorkspaceRepository>();
            services.AddSingleton<IUserActivityRepository, UserActivityRepository>();

            // TODO: Move this service out of this method.
            services.AddTransient<ISchemaMigrator, SchemaMigrator>();

            // Internal service.
            services.AddSingleton<IMapperFactory, MapperFactory>();

            services.AddSingleton<IResumeDataMapper, AddressMapper>();
            services.AddSingleton<IResumeDataMapper, CertificateMapper>();
            services.AddSingleton<IResumeDataMapper, EducationMapper>();
            services.AddSingleton<IResumeDataMapper, EmailMapper>();
            services.AddSingleton<IResumeDataMapper, ExperienceMapper>();
            services.AddSingleton<IResumeDataMapper, GitHubMapper>();
            services.AddSingleton<IResumeDataMapper, HeadlineMapper>();
            services.AddSingleton<IResumeDataMapper, LanguageMapper>();
            services.AddSingleton<IResumeDataMapper, LinkedInMapper>();
            services.AddSingleton<IResumeDataMapper, MobileMapper>();
            services.AddSingleton<IResumeDataMapper, NameMapper>();
            services.AddSingleton<IResumeDataMapper, PhotoMapper>();
            services.AddSingleton<IResumeDataMapper, SkillMapper>();
            services.AddSingleton<IResumeDataMapper, SummaryMapper>();
            services.AddSingleton<IResumeDataMapper, WebsiteMapper>();

            services.AddSingleton<IResumeDataPayloadManager, Cloudsume.Cassandra.ResumeDataPayloadManagers.PhotoPayloadManager>();

            services.AddSingleton<ISampleDataPayloadManager, Cloudsume.Cassandra.SampleDataPayloadManagers.PhotoPayloadManager>();
        }
    }
}
