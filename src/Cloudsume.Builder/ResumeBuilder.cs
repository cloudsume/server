namespace Cloudsume.Builder;

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Antlr.Runtime;
using Candidate.Server.Resume;
using Candidate.Server.Resume.Builder;
using Candidate.Server.Resume.Builder.Renderers;
using Candidate.Server.Resume.Data;
using Cloudsume.Builder.Renderers;
using Cloudsume.Resume;
using Cloudsume.Resume.Data;
using Cloudsume.Services.Client;
using NetTemplate;
using Ultima.Extensions.Collections;
using Ultima.Extensions.Globalization;
using Ultima.Extensions.Telephony;

public sealed class ResumeBuilder : IResumeBuilder
{
    private readonly IDataActionCollection<IAttributeFactory> attributes;

    public ResumeBuilder(IDataActionCollection<IAttributeFactory> attributes)
    {
        this.attributes = attributes;
    }

    public async Task<string> BuildAsync(
        string template,
        CultureInfo culture,
        IKeyedByTypeCollection<TemplateRenderOptions> options,
        IEnumerable<ResumeData> data,
        ILatexJobPoster job,
        CancellationToken cancellationToken = default)
    {
        // Setup engine.
        var listener = new TemplateErrorListener();
        var group = new TemplateGroupString("main.stg", template)
        {
            Listener = listener,
        };

        group.RegisterRenderer(typeof(SubdivisionCode), new SubdivisionCodeRenderer(culture));
        group.RegisterRenderer(typeof(Candidate.Server.Resume.Builder.Values.EducationDescription), new EducationDescriptionRenderer(culture, options));
        group.RegisterRenderer(typeof(Candidate.Server.Resume.Builder.ExperienceDescription), new ExperienceDescriptionRenderer(culture, options));
        group.RegisterRenderer(typeof(IELTS), new IELTSRenderer(culture));
        group.RegisterRenderer(typeof(ILR), new ILRRenderer(culture));
        group.RegisterRenderer(typeof(Values.Language), new LanguageRenderer(culture));
        group.RegisterRenderer(typeof(RegionInfo), new RegionInfoRenderer(culture));
        group.RegisterRenderer(typeof(TelephoneNumber), new Candidate.Server.Resume.Builder.TelephoneNumberRenderer(culture));
        group.RegisterRenderer(typeof(TexString), new TexStringRenderer(culture));
        group.RegisterRenderer(typeof(TOEFL), new TOEFLRenderer(culture));
        group.RegisterRenderer(typeof(TOEIC), new TOEICRenderer(culture));
        group.RegisterRenderer(typeof(Candidate.Server.Resume.Builder.Values.Month), new MonthRenderer(culture));
        group.RegisterRenderer(typeof(Candidate.Server.Resume.Builder.Values.Year), new YearRenderer(culture));

        var resume = group.GetInstanceOf("resume");

        if (resume == null)
        {
            throw new TemplateNotFoundException("resume");
        }

        // Build data for engine.
        var attributes = new Dictionary<string, object?>();
        var context = new BuildContext(culture, job, options);

        foreach (var g in data.GroupBy(d => d.Type))
        {
            attributes.Add(g.Key, await this.attributes[g.Key].CreateAsync(context, g, cancellationToken));
        }

        resume.Add("data", attributes);

        // Render.
        var latex = resume.Render(culture);

        if (listener.Errors.Count > 0)
        {
            var error = listener.Errors[0];
            int? line = error.Cause is RecognitionException ex ? ex.Line : null;

            throw new TemplateSyntaxException(error.ToString(), line, error.Cause);
        }

        return latex;
    }
}
