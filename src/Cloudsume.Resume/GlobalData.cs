namespace Cloudsume.Resume;

using System.Globalization;
using Candidate.Server.Resume;

public abstract class GlobalData
{
    protected GlobalData(CultureInfo culture)
    {
        this.Culture = culture;
    }

    public CultureInfo Culture { get; }

    public abstract ResumeData Data { get; }

    public void Deconstruct(out CultureInfo culture, out ResumeData data)
    {
        culture = this.Culture;
        data = this.Data;
    }
}

public sealed class GlobalData<T> : GlobalData where T : ResumeData
{
    public GlobalData(CultureInfo culture, T data)
        : base(culture)
    {
        this.Data = data;
    }

    public override T Data { get; }

    public void Deconstruct(out CultureInfo culture, out T data)
    {
        culture = this.Culture;
        data = this.Data;
    }
}
