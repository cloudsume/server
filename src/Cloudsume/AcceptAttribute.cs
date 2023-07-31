namespace Candidate.Server
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    [AttributeUsage(AttributeTargets.Property)]
    public sealed class AcceptAttribute : ValidationAttribute
    {
        public AcceptAttribute(params string[] allowed)
        {
            this.Allowed = new HashSet<string>(allowed);
        }

        public IReadOnlySet<string> Allowed { get; }

        public override bool IsValid(object? value)
        {
            var s = value as string;

            if (s == null)
            {
                return true;
            }

            return this.Allowed.Contains(s);
        }
    }
}
