namespace Cloudsume.Models
{
    using System;

    [AttributeUsage(AttributeTargets.Class)]
    public sealed class CorrespondingDomainAttribute : Attribute
    {
        public CorrespondingDomainAttribute(Type type)
        {
            this.Type = type;
        }

        public Type Type { get; }
    }
}
