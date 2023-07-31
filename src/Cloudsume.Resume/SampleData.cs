namespace Cloudsume.Resume;

using System;
using System.Globalization;
using Candidate.Server.Resume;

public class SampleData
{
    public SampleData(Guid targetJob, CultureInfo culture, ResumeData data, Guid? parentJob)
    {
        this.TargetJob = targetJob;
        this.Culture = culture;
        this.Data = data;
        this.ParentJob = parentJob;
    }

    public Guid TargetJob { get; }

    public CultureInfo Culture { get; }

    public ResumeData Data { get; set; }

    public Guid? ParentJob { get; }
}

public sealed class SampleData<T> : SampleData where T : ResumeData
{
    public SampleData(Guid targetJob, CultureInfo culture, T data, Guid? parentJob)
        : base(targetJob, culture, data, parentJob)
    {
    }

    public new T Data
    {
        get => (T)base.Data;
        set => base.Data = value;
    }
}
