namespace Cloudsume.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Ultima.Extensions.Collections;
    using Domain = Cloudsume.Resume.TemplateWorkspace;
    using ResumeDataAttribute = Cloudsume.Resume.ResumeDataAttribute;

    public sealed class TemplateWorkspace
    {
        public TemplateWorkspace(Domain domain)
        {
            this.PreviewJob = domain.PreviewJob;
            this.ApplicableData = domain.ApplicableData;
            this.RenderOptions = ToDto(domain.RenderOptions);
            this.Assets = domain.Assets.Select(a => new TemplateAsset(a)).ToArray();
        }

        public Guid? PreviewJob { get; }

        public IEnumerable<string> ApplicableData { get; }

        public IReadOnlyDictionary<string, object> RenderOptions { get; }

        public IEnumerable<TemplateAsset> Assets { get; }

        private static IReadOnlyDictionary<string, object> ToDto(IKeyedByTypeCollection<Cloudsume.Resume.TemplateRenderOptions> options)
        {
            var dto = new Dictionary<string, object>();

            foreach (var item in options)
            {
                var key = ((ResumeDataAttribute)item.TargetData.GetCustomAttributes(typeof(ResumeDataAttribute), false).Single()).Type;
                var value = GetOptionDto(item);

                dto.Add(key, value);
            }

            return dto;
        }

        private static object GetOptionDto(object domain) => domain switch
        {
            Candidate.Server.Resume.Data.EducationRenderOptions d => new EducationOptions(d),
            Candidate.Server.Resume.Data.ExperienceRenderOptions d => new ExperienceOptions(d),
            Candidate.Server.Resume.Data.SkillRenderOptions d => new SkillOptions(d),
            _ => throw new ArgumentException($"Unknow option {domain.GetType()}.", nameof(domain)),
        };
    }
}
