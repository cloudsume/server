namespace FeatureGate;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

public sealed class FeatureRegistryBuilder
{
    private readonly Dictionary<Guid, Feature> features;

    public FeatureRegistryBuilder()
    {
        this.features = new();
    }

    /// <summary>
    /// Add a new feature to experiment.
    /// </summary>
    /// <param name="id">
    /// Unique identifier of the feature.
    /// </param>
    /// <param name="name">
    /// Display name of the feature.
    /// </param>
    /// <returns>
    /// A <see cref="FeatureBuilder"/> to configure the feature.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// The feature with <paramref name="id"/> is already exists.
    /// </exception>
    /// <remarks>
    /// Once the feature is stabilized delete the feature identifier and its references to make it available to all users. You can also drop the whole feature
    /// instead of stabilized it by follow the identifier references to see the code you need to remove.
    ///
    /// The added feature will be available to all users by default. Use <see cref="FeatureBuilder.AddTargetPolicy{T}"/> to restrict the users who can use the
    /// feature.
    /// </remarks>
    public FeatureBuilder AddFeature(Guid id, string name)
    {
        var feature = new Feature(id, name);

        if (!this.features.TryAdd(id, feature))
        {
            throw new ArgumentException("The feature with specified identifier is already exists.", nameof(id));
        }

        return new(feature);
    }

    public IFeatureRegistry Build(IServiceProvider services)
    {
        // Instantiate the target policies.
        var targetPolicies = new Dictionary<Type, TargetPolicy>();

        foreach (var type in this.features.Values.SelectMany(f => f.TargetPolicies))
        {
            // Check if the policy already instantiated.
            ref var policy = ref CollectionsMarshal.GetValueRefOrAddDefault(targetPolicies, type, out var exists);

            if (exists)
            {
                continue;
            }

            // Instantiate the policy.
            object? service;

            try
            {
                service = services.GetService(type);

                if (service is null)
                {
                    throw new ArgumentException($"Service {type} is not available.", nameof(services));
                }

                policy = (TargetPolicy)service;
            }
            catch
            {
                targetPolicies.Remove(type);
                throw;
            }
        }

        return new FeatureRegistry(this.features, targetPolicies);
    }
}
