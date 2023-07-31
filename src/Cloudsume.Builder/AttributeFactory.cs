namespace Cloudsume.Builder;

using System;
using System.Globalization;
using Ultima.Extensions.Primitives;

internal abstract class AttributeFactory
{
    protected static RegionInfo? GetCountry(BuildContext context, string? code)
    {
        if (code == null)
        {
            return null;
        }
        else
        {
            var region = new RegionInfo(code);

            return region.Equals(new RegionInfo(context.Culture.Name)) ? null : region;
        }
    }

    protected static object? GetDate(DateTime? value)
    {
        if (value is DateTime v)
        {
            return new
            {
                Month = Candidate.Server.Resume.Builder.Values.Month.From(v),
                Year = Candidate.Server.Resume.Builder.Values.Year.From(v),
            };
        }
        else
        {
            return null;
        }
    }

    protected static object? GetDate(YearMonth? value) => GetDate(value is { } v ? new DateTime(v.Year, v.Month, 1) : null);
}
