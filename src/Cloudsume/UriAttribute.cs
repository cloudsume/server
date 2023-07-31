namespace Cloudsume
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter)]
    public sealed class UriAttribute : ValidationAttribute
    {
        public UriSchemes Schemes { get; set; }

        public UriKind Kind { get; set; }

        public override bool IsValid(object? value)
        {
            var uri = value as Uri;

            if (uri == null)
            {
                return true;
            }

            // Check schemes.
            var schemes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            if (this.Schemes.HasFlag(UriSchemes.HTTP))
            {
                schemes.Add("http");
            }

            if (this.Schemes.HasFlag(UriSchemes.HTTPS))
            {
                schemes.Add("https");
            }

            if (schemes.Count > 0 && !schemes.Contains(uri.Scheme))
            {
                return false;
            }

            // Check kind.
            switch (this.Kind)
            {
                case UriKind.Absolute:
                    if (!uri.IsAbsoluteUri)
                    {
                        return false;
                    }

                    break;
                case UriKind.Relative:
                    if (uri.IsAbsoluteUri)
                    {
                        return false;
                    }

                    break;
            }

            return true;
        }
    }
}
