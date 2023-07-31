namespace FeatureGate;

using System.Security.Claims;

public abstract class TargetPolicy
{
    /// <summary>
    /// Check if the specified user meet with this policy.
    /// </summary>
    /// <param name="target">
    /// The user to check.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="target"/> meet with this policy otherwise <see langword="false"/>.
    /// </returns>
    public abstract bool Run(ClaimsPrincipal target);
}
