namespace Cloudsume;

using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

internal sealed class AuthorizationRequirement : AuthorizationHandler<AuthorizationRequirement>, IAuthorizationRequirement
{
    private readonly AuthorizationPolicyAttribute policy;

    public AuthorizationRequirement(AuthorizationPolicyAttribute policy)
    {
        this.policy = policy;
    }

    public override string ToString()
    {
        var policy = this.policy;

        return $"Ultima Account Scope = {policy.UltimaScope}, Allow Guest = {policy.AllowGuest}";
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AuthorizationRequirement requirement)
    {
        // No need to check if user is authenticated because we use AuthorizationPolicyBuilder.RequireAuthenticatedUser().
        if (context.User is not { } user)
        {
            return Task.CompletedTask;
        }

        // Check token issuer.
        var issuer = user.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Iss);
        var policy = requirement.policy;

        if (issuer?.Value == "cloudsume")
        {
            // Guest account.
            if (policy.AllowGuest)
            {
                context.Succeed(requirement);
            }
        }
        else
        {
            // Ultima Account.
            foreach (var c in user.Claims)
            {
                if (!string.Equals(c.Type, "scope", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (!string.Equals(c.Value, policy.UltimaScope, StringComparison.Ordinal))
                {
                    continue;
                }

                context.Succeed(requirement);
                break;
            }
        }

        return Task.CompletedTask;
    }
}
