namespace FeatureGate;

using System;
using System.Collections.Generic;
using System.Security.Claims;

public interface IFeatureRegistry
{
    /// <summary>
    /// Check if the target user can use the specified feature.
    /// </summary>
    /// <param name="featureId">
    /// Identifier of the feature to check.
    /// </param>
    /// <param name="target">
    /// Target user.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="target"/> is allowed to use <paramref name="featureId"/> feature otherwise <see langword="false"/>.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// <paramref name="featureId"/> is not a valid feature identifier.
    /// </exception>
    bool IsAllowed(Guid featureId, ClaimsPrincipal target);

    /// <summary>
    /// Gets all features that the specified user can be using.
    /// </summary>
    /// <param name="target">
    /// Target user.
    /// </param>
    /// <returns>
    /// A list of features that the user can be using.
    /// </returns>
    IEnumerable<Feature> GetFeaturesFor(ClaimsPrincipal target);
}
