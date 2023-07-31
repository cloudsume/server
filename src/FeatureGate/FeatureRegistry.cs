namespace FeatureGate;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

internal sealed class FeatureRegistry : IFeatureRegistry
{
    private readonly Dictionary<Guid, Feature> features;
    private readonly Dictionary<Type, TargetPolicy> targetPolicies;

    public FeatureRegistry(Dictionary<Guid, Feature> features, Dictionary<Type, TargetPolicy> targetPolicies)
    {
        this.features = features;
        this.targetPolicies = targetPolicies;
    }

    public IEnumerable<Feature> GetFeaturesFor(ClaimsPrincipal target)
    {
        var features = new List<Feature>();

        foreach (var feature in this.features.Values)
        {
            if (this.IsAllowed(feature, target))
            {
                features.Add(feature);
            }
        }

        return features;
    }

    public bool IsAllowed(Guid featureId, ClaimsPrincipal target)
    {
        if (!this.features.TryGetValue(featureId, out var feature))
        {
            throw new ArgumentException("The value is not a valid feature identifier.", nameof(featureId));
        }

        return this.IsAllowed(feature, target);
    }

    private bool IsAllowed(Feature feature, ClaimsPrincipal target)
    {
        // Make the feature available to all users if no target policy.
        var policies = feature.TargetPolicies;

        if (policies.Count == 0)
        {
            return true;
        }

        // Evaluate the policies.
        foreach (var policy in policies.Select(p => this.targetPolicies[p]))
        {
            if (policy.Run(target))
            {
                return true;
            }
        }

        return false;
    }
}
