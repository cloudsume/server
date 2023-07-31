namespace Cloudsume.Cassandra.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using global::Cassandra;

public abstract class DataObject
{
    public static readonly IReadOnlyDictionary<Type, string> DomainTypes = typeof(DataObject)
        .Assembly
        .GetTypes()
        .Where(t => !t.IsAbstract && typeof(DataObject).IsAssignableFrom(t))
        .ToDictionary(t => t, GetDomainType);

    public DateTimeOffset UpdatedTime { get; set; }

    protected static UdtMap<T> CreateMapping<T>(string name) where T : DataObject, new() => UdtMap.For<T>(name)
        .Map(d => d.UpdatedTime, "updated");

    private static string GetDomainType(Type model)
    {
        var domainAttr = model.GetCustomAttribute<DomainTypeAttribute>(false);

        if (domainAttr == null)
        {
            throw new Exception($"No attribute '{typeof(DomainTypeAttribute)}' has been applied on '{model}'.");
        }

        // Get ResumeDataAttribute.
        var domain = domainAttr.Value;
        var data = domain.GetCustomAttribute<Cloudsume.Resume.ResumeDataAttribute>(false);

        if (!typeof(Candidate.Server.Resume.ResumeData).IsAssignableFrom(domain) || data == null)
        {
            throw new Exception($"The type specified on '{typeof(DomainTypeAttribute)}' for '{model}' is not a resume data.");
        }

        return data.Type;
    }
}
