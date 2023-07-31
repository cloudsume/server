namespace Cloudsume.Options
{
    using System.Collections.Generic;

    // The cofngiruation library does not support key type other than string and enum.
    public sealed class TemplatePriceConstraintOptions : Dictionary<string, TemplatePriceConstraint>
    {
    }
}
