namespace Cloudsume.Cassandra;

using System;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
internal sealed class DomainTypeAttribute : Attribute
{
    public DomainTypeAttribute(Type value)
    {
        this.Value = value;
    }

    public Type Value { get; }
}
