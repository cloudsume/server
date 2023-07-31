namespace Cloudsume.Resume;

using System;

[AttributeUsage(AttributeTargets.Property)]
public sealed class DataPropertyAttribute : Attribute
{
    public DataPropertyAttribute(string id)
    {
        this.Id = id;
    }

    public string Id { get; }
}
