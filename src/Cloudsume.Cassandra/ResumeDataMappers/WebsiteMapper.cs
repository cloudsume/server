namespace Cloudsume.Cassandra.ResumeDataMappers;

using System;
using Cloudsume.Cassandra.Models;
using Domain = Cloudsume.Resume.Data.Website;

internal sealed class WebsiteMapper : ResumeDataMapper<Domain, WebsiteData>
{
    public override WebsiteData ToCassandra(Domain domain) => new()
    {
        Url = TextProperty.From(domain.Value, v => v?.AbsoluteUri),
        UpdatedTime = domain.UpdatedAt,
    };

    public override Domain ToDomain(Guid id, Guid? parent, WebsiteData cassandra)
    {
        var url = cassandra.Url.ToDomain(v =>
        {
            if (v is null)
            {
                return null;
            }

            try
            {
                return new Uri(v);
            }
            catch (UriFormatException ex)
            {
                throw new ArgumentException($"The value of {nameof(cassandra.Url)} is not a valid URI.", nameof(cassandra), ex);
            }
        });

        return new(url, cassandra.UpdatedTime.LocalDateTime);
    }
}
