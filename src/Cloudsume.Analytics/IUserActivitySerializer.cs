namespace Cloudsume.Analytics;

using System;
using System.Net;
using NetUlid;

public interface IUserActivitySerializer
{
    (Guid Type, byte[] Data) Serialize(UserActivity activity);

    UserActivity Deserialize(Guid userId, Ulid id, Guid type, ReadOnlyMemory<byte> data, IPAddress ipAddress, string userAgent);
}
