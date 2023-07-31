namespace Cloudsume;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using Cloudsume.Analytics;
using NetUlid;
using Factory = System.Func<System.Guid, NetUlid.Ulid, System.ReadOnlyMemory<byte>, System.Net.IPAddress, string, Cloudsume.Analytics.UserActivity>;

internal sealed class UserActivitySerializer : IUserActivitySerializer
{
    private readonly IReadOnlyDictionary<Type, Guid> typeIds;
    private readonly IReadOnlyDictionary<Guid, Factory> factories;

    public UserActivitySerializer()
    {
        var (typeIds, factories) = BuildTables(this.GetType().Assembly);

        this.typeIds = typeIds;
        this.factories = factories;
    }

    public UserActivity Deserialize(Guid userId, Ulid id, Guid type, ReadOnlyMemory<byte> data, IPAddress ipAddress, string userAgent)
    {
        if (!this.factories.TryGetValue(type, out var factory))
        {
            throw new ArgumentException("Unknown data type.", nameof(type));
        }

        return factory.Invoke(userId, id, data, ipAddress, userAgent);
    }

    public (Guid Type, byte[] Data) Serialize(UserActivity activity) => (this.typeIds[activity.GetType()], activity.Serialize());

    private static (Dictionary<Type, Guid> TypeIds, Dictionary<Guid, Factory> Factories) BuildTables(Assembly assembly)
    {
        var typeIds = new Dictionary<Type, Guid>();
        var factories = new Dictionary<Guid, Factory>();

        foreach (var type in assembly.GetTypes().Where(t => !t.IsAbstract && t.IsAssignableTo(typeof(UserActivity))))
        {
            // Get Deserialize method.
            var deserialize = type.GetMethod("Deserialize", BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Static);

            if (deserialize is null)
            {
                throw new ArgumentException($"No Deserialize static method has been defined on {type}.", nameof(assembly));
            }

            // Check return type.
            if (deserialize.ReturnType != type)
            {
                throw new ArgumentException($"{deserialize} has incorrect return type.", nameof(assembly));
            }

            // Check parameters.
            var parameters = deserialize.GetParameters().Select(p => p.ParameterType).ToArray();

            if (!parameters.SequenceEqual(new[] { typeof(Guid), typeof(Ulid), typeof(ReadOnlyMemory<byte>), typeof(IPAddress), typeof(string) }))
            {
                throw new ArgumentException($"{deserialize} has incorrect parameters.", nameof(assembly));
            }

            // Add to the table.
            var id = type.GUID;

            typeIds.Add(type, id);
            factories.Add(id, BuildFactory(deserialize));
        }

        return (typeIds, factories);
    }

    private static Factory BuildFactory(MethodInfo method)
    {
        var userId = Expression.Parameter(typeof(Guid));
        var id = Expression.Parameter(typeof(Ulid));
        var data = Expression.Parameter(typeof(ReadOnlyMemory<byte>));
        var ip = Expression.Parameter(typeof(IPAddress));
        var ua = Expression.Parameter(typeof(string));
        var call = Expression.Call(method, userId, id, data, ip, ua);
        var cast = Expression.Convert(call, typeof(UserActivity));
        var factory = Expression.Lambda<Factory>(cast, userId, id, data, ip, ua);

        return factory.Compile();
    }
}
