namespace Cloudsume;

using System;

[AttributeUsage(AttributeTargets.Field)]
public sealed class AuthorizationPolicyAttribute : Attribute
{
    public AuthorizationPolicyAttribute(string ultimaScope)
    {
        this.UltimaScope = ultimaScope;
        this.AllowGuest = false;
    }

    public string UltimaScope { get; }

    public bool AllowGuest { get; set; }
}
