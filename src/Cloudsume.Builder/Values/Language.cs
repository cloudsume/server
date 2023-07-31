namespace Cloudsume.Builder.Values;

using System.Globalization;
using Candidate.Server.Resume.Builder;

internal sealed class Language : AttributeValue<Language, CultureInfo>
{
    public Language(CultureInfo value)
        : base(value)
    {
    }
}
