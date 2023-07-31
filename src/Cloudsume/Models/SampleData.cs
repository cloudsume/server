namespace Cloudsume.Models;

using System;

public sealed class SampleData
{
    public SampleData(Guid targetJob, string locale, ResumeData data, Guid? parentJob)
    {
        this.TargetJob = targetJob;
        this.Locale = locale;
        this.Data = data;
        this.ParentJob = parentJob;
    }

    public Guid TargetJob { get; }

    public string Locale { get; }

    public ResumeData Data { get; }

    public Guid? ParentJob { get; }
}
