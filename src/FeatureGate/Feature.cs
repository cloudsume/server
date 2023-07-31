namespace FeatureGate;

using System;
using System.Collections.Generic;
using System.Globalization;
using Candidate.Globalization;

public sealed class Feature
{
    private readonly TranslationCollection name;
    private readonly List<Type> targetPolicies;

    internal Feature(Guid id, string name)
    {
        this.Id = id;
        this.name = new()
        {
            { string.Empty, name },
        };

        this.targetPolicies = new();
    }

    public Guid Id { get; }

    public ITranslationCollection Name => this.name;

    internal IReadOnlyCollection<Type> TargetPolicies => this.TargetPolicies;

    internal void AddName(CultureInfo culture, string name) => this.name.Add(culture, name);

    internal void AddTargetPolicy(Type policy) => this.targetPolicies.Add(policy);
}
