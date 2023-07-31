namespace Cloudsume
{
    using System.Collections.Generic;

    public sealed class CorsOptions
    {
        public IEnumerable<string>? Origins { get; set; }
    }
}
