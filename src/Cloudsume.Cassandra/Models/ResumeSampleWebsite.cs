namespace Cloudsume.Cassandra.Models;

using global::Cassandra.Mapping;

public sealed class ResumeSampleWebsite : ResumeSampleData<ResumeSampleWebsite, WebsiteData>
{
    public static readonly ITypeDefinition Mapping = CreateMapping("resume_sample_websites");
}
