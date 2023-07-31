namespace Cloudsume.Models;

using System.Globalization;

public sealed class GlobalData
{
    public GlobalData(CultureInfo culture, ResumeData data)
    {
        this.Culture = culture;
        this.Data = data;
    }

    public CultureInfo Culture { get; }

    public ResumeData Data { get; }
}
