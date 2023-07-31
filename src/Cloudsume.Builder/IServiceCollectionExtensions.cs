namespace Microsoft.Extensions.DependencyInjection
{
    using Cloudsume.Builder;
    using Cloudsume.Builder.AttributeFactories;
    using Cloudsume.Resume;

    public static class IServiceCollectionExtensions
    {
        public static void AddResumeBuilder(this IServiceCollection services)
        {
            // Public services.
            services.AddSingleton<IResumeBuilder, ResumeBuilder>();

            // Internal services.
            services.AddSingleton<IAttributeFactory, AddressAttributeFactory>();
            services.AddSingleton<IAttributeFactory, CertificateAttributeFactory>();
            services.AddSingleton<IAttributeFactory, EducationAttributeFactory>();
            services.AddSingleton<IAttributeFactory, EmailAddressAttributeFactory>();
            services.AddSingleton<IAttributeFactory, ExperienceAttributeFactory>();
            services.AddSingleton<IAttributeFactory, GitHubAttributeFactory>();
            services.AddSingleton<IAttributeFactory, HeadlineAttributeFactory>();
            services.AddSingleton<IAttributeFactory, LanguageAttributeFactory>();
            services.AddSingleton<IAttributeFactory, LinkedInAttributeFactory>();
            services.AddSingleton<IAttributeFactory, MobileAttributeFactory>();
            services.AddSingleton<IAttributeFactory, NameAttributeFactory>();
            services.AddSingleton<IAttributeFactory, PhotoAttributeFactory>();
            services.AddSingleton<IAttributeFactory, SkillAttributeFactory>();
            services.AddSingleton<IAttributeFactory, SummaryAttributeFactory>();
            services.AddSingleton<IAttributeFactory, WebsiteAttributeFactory>();
        }
    }
}
