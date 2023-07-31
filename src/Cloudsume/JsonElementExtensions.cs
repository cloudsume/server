namespace Cloudsume
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Text.Json;

    internal static class JsonElementExtensions
    {
        public static T GetEnum<T>(this in JsonElement json, string property) where T : struct, Enum
        {
            if (!json.TryGetProperty(property, out var value))
            {
                throw new JsonException($"Property '{property}' is required.");
            }
            else if (value.ValueKind != JsonValueKind.Number)
            {
                throw new JsonException($"The value of property '{property}' is not valid.");
            }

            try
            {
                return EnumTraits<T>.Underlying switch
                {
                    TypeCode.SByte => GetEnumFromSByte<T>(value),
                    TypeCode.Byte => GetEnumFromByte<T>(value),
                    TypeCode.Int16 => GetEnumFromInt16<T>(value),
                    TypeCode.Int32 => GetEnumFromInt32<T>(value),
                    TypeCode.Int64 => GetEnumFromInt64<T>(value),
                    _ => throw new NotImplementedException($"The underlying type of enum '{typeof(T)}' is not supported yet."),
                };
            }
            catch (FormatException ex)
            {
                throw new JsonException($"The value of property '{property}' is not valid.", ex);
            }
        }

        public static string GetString(this in JsonElement json, string property)
        {
            if (!json.TryGetProperty(property, out var value))
            {
                throw new JsonException($"Property '{property}' is required.");
            }
            else if (value.ValueKind != JsonValueKind.String)
            {
                throw new JsonException($"The value of property '{property}' is not valid.");
            }

            return value.GetString()!;
        }

        private static T GetEnumFromSByte<T>(in JsonElement json) where T : struct, Enum
        {
            var value = json.GetSByte();

            return Unsafe.As<sbyte, T>(ref value);
        }

        private static T GetEnumFromByte<T>(in JsonElement json) where T : struct, Enum
        {
            var value = json.GetByte();

            return Unsafe.As<byte, T>(ref value);
        }

        private static T GetEnumFromInt16<T>(in JsonElement json) where T : struct, Enum
        {
            var value = json.GetInt16();

            return Unsafe.As<short, T>(ref value);
        }

        private static T GetEnumFromInt32<T>(in JsonElement json) where T : struct, Enum
        {
            var value = json.GetInt32();

            return Unsafe.As<int, T>(ref value);
        }

        private static T GetEnumFromInt64<T>(in JsonElement json) where T : struct, Enum
        {
            var value = json.GetInt64();

            return Unsafe.As<long, T>(ref value);
        }

        private static class EnumTraits<T> where T : struct, Enum
        {
            public static readonly TypeCode Underlying = default(T).GetTypeCode();
        }
    }
}
