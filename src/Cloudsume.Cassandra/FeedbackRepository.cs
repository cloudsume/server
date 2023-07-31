namespace Cloudsume.Cassandra;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Cloudsume.Analytics;
using Cornot;
using NetUlid;

internal sealed class FeedbackRepository : IFeedbackRepository
{
    private readonly IMapperFactory db;

    public FeedbackRepository(IMapperFactory db)
    {
        this.db = db;
    }

    public Task CreateAsync(Feedback feedback, CancellationToken cancellationToken = default)
    {
        var db = this.db.CreateMapper();
        var row = new Models.Feedback()
        {
            Id = feedback.Id.ToByteArray(),
            Score = feedback.Score is { } score ? Convert.ToSByte(score) : (sbyte)-1,
            Detail = string.IsNullOrWhiteSpace(feedback.Detail) ? null : feedback.Detail,
            Contact = feedback.Contact?.Address,
            User = feedback.UserId,
            IpAddress = feedback.IpAddress,
            UserAgent = feedback.UserAgent,
        };

        return db.InsertAsync(row);
    }

    public async Task<IEnumerable<Feedback>> ListAsync(int? score, Ulid? skipTill, int limit, CancellationToken cancellationToken = default)
    {
        if (limit <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(limit));
        }

        // Build the query.
        var query = new StringBuilder("FROM feedbacks_by_score WHERE score = ?");
        var @params = new List<object>();

        if (score is null)
        {
            @params.Add((sbyte)-1);
        }
        else
        {
            @params.Add(Convert.ToSByte(score.Value));
        }

        if (skipTill is { } skip)
        {
            query.Append(" AND id < ?");
            @params.Add(skip.ToByteArray());
        }

        query.Append(" ORDER BY id DESC LIMIT ?");
        @params.Add(limit);

        // Fetch.
        var db = this.db.CreateMapper();
        var rows = await db.FetchAsync<Models.Feedback>(query.ToString(), @params.ToArray());

        return rows.Select(this.ToDomain).ToArray();
    }

    private Feedback ToDomain(Models.Feedback row) => new(
        new(row.Id),
        row.Score == -1 ? null : row.Score,
        row.Detail ?? string.Empty,
        row.Contact is { } contact ? new(contact) : null,
        row.User,
        row.IpAddress ?? throw new DataCorruptionException(row, $"{nameof(row.IpAddress)} is null."),
        row.UserAgent ?? string.Empty);
}
