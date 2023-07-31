namespace Cloudsume
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;

    public sealed class ApplicationOptions
    {
        public ApplicationOptions()
        {
            this.AllowedTemplateCultures = new List<CultureInfo>();
        }

        [MinLength(1)]
        public IList<CultureInfo> AllowedTemplateCultures { get; }

        [Range(1, int.MaxValue)]
        public int MaximumResumePerUser { get; set; }
    }
}
