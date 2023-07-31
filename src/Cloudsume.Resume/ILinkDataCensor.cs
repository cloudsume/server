namespace Cloudsume.Resume;

using System.Collections.Generic;
using System.Globalization;
using Candidate.Server.Resume;

public interface ILinkDataCensor
{
    IEnumerable<ResumeData> Run(IEnumerable<ResumeData> data, IReadOnlySet<LinkCensorship> censorships, CultureInfo culture);
}
