namespace Cloudsume.Resume;

using System;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class ResumeDataAttribute : Attribute
{
    public ResumeDataAttribute(string type)
    {
        this.Type = type;
    }

    public string Type { get; }
}
