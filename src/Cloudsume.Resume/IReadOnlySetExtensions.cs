namespace Cloudsume.Resume;

using System.Collections.Generic;

public static class IReadOnlySetExtensions
{
    public static bool Contains(this IReadOnlySet<LinkCensorship> set, string data, string property)
    {
        var key = new LinkCensorship(data, property);

        return set.Contains(key);
    }
}
