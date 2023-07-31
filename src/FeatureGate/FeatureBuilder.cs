namespace FeatureGate;

using System;
using System.Globalization;

public sealed class FeatureBuilder
{
    private readonly Feature feature;

    internal FeatureBuilder(Feature feature)
    {
        this.feature = feature;
    }

    public FeatureBuilder AddName(string culture, string name)
    {
        CultureInfo cultureInfo;

        try
        {
            cultureInfo = CultureInfo.GetCultureInfo(culture);
        }
        catch (CultureNotFoundException ex)
        {
            throw new ArgumentException("The value is not a valid culture name.", nameof(culture), ex);
        }

        return this.AddName(cultureInfo, name);
    }

    public FeatureBuilder AddName(CultureInfo culture, string name)
    {
        this.feature.AddName(culture, name);
        return this;
    }

    public FeatureBuilder AddTargetPolicy<T>() where T : TargetPolicy => this.AddTargetPolicy(typeof(T));

    public FeatureBuilder AddTargetPolicy(Type policy)
    {
        this.feature.AddTargetPolicy(policy);
        return this;
    }
}
