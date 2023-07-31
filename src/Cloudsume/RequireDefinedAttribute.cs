namespace Candidate.Server
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter)]
    public sealed class RequireDefinedAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value == null)
            {
                return true;
            }

            var type = value.GetType();

            if (!type.IsEnum)
            {
                return true;
            }

            if (type.CustomAttributes.Any(d => typeof(FlagsAttribute).IsAssignableFrom(d.AttributeType)))
            {
                var u = type.GetEnumUnderlyingType();

                if (u == typeof(sbyte) || u == typeof(short) || u == typeof(int) || u == typeof(long) || u == typeof(nint))
                {
                    var flags = Convert.ToInt64(value);
                    long valids = 0;

                    foreach (var v in Enum.GetValues(type))
                    {
                        valids |= Convert.ToInt64(v);
                    }

                    return (valids | flags) == valids;
                }
                else if (u == typeof(byte) || u == typeof(ushort) || u == typeof(uint) || u == typeof(ulong) || u == typeof(nuint))
                {
                    var flags = Convert.ToUInt64(value);
                    ulong valids = 0;

                    foreach (var v in Enum.GetValues(type))
                    {
                        valids |= Convert.ToUInt64(v);
                    }

                    return (valids | flags) == valids;
                }
                else
                {
                    throw new ArgumentException($"Unknow underlying type {u} for {type}.", nameof(value));
                }
            }
            else
            {
                return type.IsEnumDefined(value);
            }
        }
    }
}
