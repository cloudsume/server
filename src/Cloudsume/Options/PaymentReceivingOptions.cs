namespace Cloudsume.Options
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics.CodeAnalysis;

    public sealed class PaymentReceivingOptions
    {
        [Required]
        [MinLength(1)]
        [NotNull]
        public IReadOnlyCollection<Uri>? AllowedSetupReturnUris { get; set; }
    }
}
