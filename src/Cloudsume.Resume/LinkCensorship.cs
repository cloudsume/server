namespace Cloudsume.Resume;

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.Json.Serialization;
using Candidate.Server.Resume;

[JsonConverter(typeof(LinkCensorshipJsonConverter))]
public sealed class LinkCensorship : IComparable<LinkCensorship>, IEquatable<LinkCensorship>
{
    private static readonly IReadOnlySet<string> ValidSet = BuildValidSet();

    public LinkCensorship(string data, string property)
    {
        if (!ValidSet.Contains($"{data}.{property}"))
        {
            throw new ArgumentException($"'{data}' is not a valid data type or '{property}' is not a valid property identifier.");
        }

        this.Data = data;
        this.Property = property;
    }

    private LinkCensorship(string id)
    {
        var sep = id.IndexOf('.');

        if (sep == -1)
        {
            throw new ArgumentException("No '.' in the value.", nameof(id));
        }

        this.Data = id[..sep];
        this.Property = id[(sep + 1)..];
    }

    public string Data { get; }

    public string Property { get; }

    public static LinkCensorship Parse(string s)
    {
        if (!ValidSet.Contains(s))
        {
            throw new FormatException($"'{s}' is not a valid link censorship");
        }

        return new(s);
    }

    public int CompareTo(LinkCensorship? other)
    {
        if (other is null)
        {
            return 1;
        }

        var data = this.Data.CompareTo(other.Data);

        if (data != 0)
        {
            return data;
        }

        return this.Property.CompareTo(other.Property);
    }

    public bool Equals(LinkCensorship? other) => other is not null && this.Data == other.Data && this.Property == other.Property;

    public override bool Equals(object? obj) => obj is LinkCensorship other && this.Equals(other);

    public override int GetHashCode() => HashCode.Combine(this.Data, this.Property);

    public override string ToString() => $"{this.Data}.{this.Property}";

    private static IReadOnlySet<string> BuildValidSet()
    {
        var set = new HashSet<string>();

        foreach (var type in typeof(ResumeData).Assembly.GetTypes())
        {
            // Check if type has ResumeDataAttribute.
            var data = type.GetCustomAttribute<ResumeDataAttribute>()?.Type;

            if (data is null)
            {
                continue;
            }

            // Enumerate properties.
            foreach (var prop in type.GetProperties())
            {
                var id = prop.GetCustomAttribute<DataPropertyAttribute>()?.Id;

                if (id is null)
                {
                    continue;
                }

                set.Add($"{data}.{id}");
            }
        }

        return set;
    }
}
