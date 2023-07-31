namespace Cloudsume.Cassandra
{
    using global::Cassandra;
    using global::Cassandra.Mapping;

    public sealed class MapperFactory : IMapperFactory
    {
        private readonly ISession session;
        private readonly MappingConfiguration mapping;

        public MapperFactory(ISession session)
        {
            this.session = session;
            this.mapping = new();

            this.session.UserDefinedTypes.Define(this.GetUdtMaps());
            this.mapping.Define(this.GetMaps());
        }

        public IMapper CreateMapper()
        {
            return new Mapper(this.session, this.mapping);
        }

        private UdtMap[] GetUdtMaps() => new[]
        {
            Models.AddressData.Mapping,
            Models.AsciiProperty.Mapping,
            Models.CertificateData.Mapping,
            Models.DateProperty.Mapping,
            Models.EducationData.Mapping,
            Models.EmailData.Mapping,
            Models.ExperienceData.Mapping,
            Models.GitHubData.Mapping,
            Models.HeadlineData.Mapping,
            Models.IeltsScore.Mapping,
            Models.IeltsScoreProperty.Mapping,
            Models.LanguageData.Mapping,
            Models.LinkedInData.Mapping,
            Models.MobileData.Mapping,
            Models.Month.Mapping,
            Models.MonthProperty.Mapping,
            Models.NameData.Mapping,
            Models.Photo.Mapping,
            Models.PhotoData.Mapping,
            Models.PhotoProperty.Mapping,
            Models.SByteProperty.Mapping,
            Models.ShortProperty.Mapping,
            Models.SkillData.Mapping,
            Models.SummaryData.Mapping,
            Models.Telephone.Mapping,
            Models.TelephoneProperty.Mapping,
            Models.TemplateAsset.Mapping,
            Models.TemplateEducationOptions.Mapping,
            Models.TemplateExperienceOptions.Mapping,
            Models.TemplateSkillOptions.Mapping,
            Models.TextProperty.Mapping,
            Models.UuidProperty.Mapping,
            Models.WebsiteData.Mapping,
        };

        private ITypeDefinition[] GetMaps() => new[]
        {
            Models.Configuration.Mapping,
            Models.Feedback.Mapping,
            Models.GuestSession.Mapping,
            Models.Job.Mapping,
            Models.PaymentReceivingMethod.Mapping,
            Models.RecruitmentRevoke.Mapping,
            Models.Resume.Mapping,
            Models.ResumeAddress.Mapping,
            Models.ResumeCertificate.Mapping,
            Models.ResumeEducation.Mapping,
            Models.ResumeEmail.Mapping,
            Models.ResumeExperience.Mapping,
            Models.ResumeGitHub.Mapping,
            Models.ResumeHeadline.Mapping,
            Models.ResumeLanguage.Mapping,
            Models.ResumeLink.Mapping,
            Models.ResumeLinkAccess.Mapping,
            Models.ResumeLinkedIn.Mapping,
            Models.ResumeMobile.Mapping,
            Models.ResumeName.Mapping,
            Models.ResumePhoto.Mapping,
            Models.ResumeSampleAddress.Mapping,
            Models.ResumeSampleCertificate.Mapping,
            Models.ResumeSampleEducation.Mapping,
            Models.ResumeSampleEmail.Mapping,
            Models.ResumeSampleExperience.Mapping,
            Models.ResumeSampleGitHub.Mapping,
            Models.ResumeSampleHeadline.Mapping,
            Models.ResumeSampleLanguage.Mapping,
            Models.ResumeSampleLinkedIn.Mapping,
            Models.ResumeSampleMobile.Mapping,
            Models.ResumeSampleName.Mapping,
            Models.ResumeSamplePhoto.Mapping,
            Models.ResumeSampleSkill.Mapping,
            Models.ResumeSampleSummary.Mapping,
            Models.ResumeSampleWebsite.Mapping,
            Models.ResumeSkill.Mapping,
            Models.ResumeSummary.Mapping,
            Models.ResumeWebsite.Mapping,
            Models.Template.Mapping,
            Models.TemplateCancelPurchaseFeedback.Mapping,
            Models.TemplateLicense.Mapping,
            Models.TemplatePack.Mapping,
            Models.TemplatePackLicense.Mapping,
            Models.TemplatePackMember.Mapping,
            Models.TemplateRegistration.Mapping,
            Models.TemplateRegistrationStats.Mapping,
            Models.TemplateStats.Mapping,
            Models.TemplateWorkspace.Mapping,
            Models.UserActivity.Mapping,
        };
    }
}
