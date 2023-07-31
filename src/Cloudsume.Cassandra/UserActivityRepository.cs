namespace Cloudsume.Cassandra;

using System.Threading;
using System.Threading.Tasks;
using Cloudsume.Analytics;

public sealed class UserActivityRepository : IUserActivityRepository
{
    private readonly IMapperFactory db;
    private readonly IUserActivitySerializer serializer;

    public UserActivityRepository(IMapperFactory db, IUserActivitySerializer serializer)
    {
        this.db = db;
        this.serializer = serializer;
    }

    public Task WriteAsync(UserActivity activity, CancellationToken cancellationToken = default)
    {
        var db = this.db.CreateMapper();
        var (type, data) = this.serializer.Serialize(activity);
        var row = new Models.UserActivity()
        {
            UserId = activity.UserId,
            Id = activity.Id.ToByteArray(),
            Type = type,
            Data = data,
            IpAddress = activity.IpAddress,
            UserAgent = string.IsNullOrEmpty(activity.UserAgent) ? null : activity.UserAgent,
        };

        return db.InsertAsync(row);
    }
}
