namespace Cloudsume.Cassandra.Models;

using global::Cassandra.Mapping;

public sealed class ResumeWebsite : ResumeData<ResumeWebsite, WebsiteData>
{
    public static readonly ITypeDefinition Mapping = CreateMapping("resume_websites");
}
