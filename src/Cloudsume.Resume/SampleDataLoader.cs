namespace Cloudsume.Resume;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Candidate.Server.Resume;
using Cornot;

internal sealed class SampleDataLoader : ISampleDataLoader
{
    private readonly ISampleDataRepository repository;

    public SampleDataLoader(ISampleDataRepository repository)
    {
        this.repository = repository;
    }

    public async Task<Tuple<IEnumerable<ResumeData>, IParentCollection>?> LoadAsync(
        Guid userId,
        Guid jobId,
        CultureInfo culture,
        string type,
        CancellationToken cancellationToken = default)
    {
        // Load top level.
        IEnumerable<SampleData> tops;
        CultureInfo current;

        for (; ;)
        {
            for (current = culture; ; current = current.Parent)
            {
                tops = await this.repository.GetAsync(userId, jobId, current, type, cancellationToken);

                if (current.Equals(CultureInfo.InvariantCulture) || tops.Any())
                {
                    break;
                }
            }

            if (tops.Any())
            {
                break;
            }
            else if (jobId == Guid.Empty)
            {
                return null;
            }

            // Try default job.
            jobId = Guid.Empty;
        }

        // Load parents.
        var parents = new ParentCollection();
        var result = new Tuple<IEnumerable<ResumeData>, IParentCollection>(tops.Select(s => s.Data).ToArray(), parents);

        for (; ;)
        {
            tops = await this.LoadParentsAsync(userId, type, tops, cancellationToken);

            if (!tops.Any())
            {
                break;
            }
            else if (parents.Count == 20)
            {
                // This should never happened on normal usage.
                throw new SampleDataLoaderException($"Too many parent layer for data {type}:{userId}:{jobId}:{current}.");
            }

            var layer = new Dictionary<GlobalKey, ResumeData>();

            foreach (var parent in tops)
            {
                var data = parent.Data;

                layer.Add(new(data), data);
            }

            parents.Add(layer);
        }

        return result;
    }

    private async Task<IEnumerable<SampleData>> LoadParentsAsync(
        Guid userId,
        string type,
        IEnumerable<SampleData> tops,
        CancellationToken cancellationToken = default)
    {
        var parents = new List<SampleData>();
        var loaded = new HashSet<(Guid Job, CultureInfo Culture)>();

        foreach (var top in tops)
        {
            var data = top.Data;

            if (!data.HasFallbacks)
            {
                continue;
            }

            var jobId = top.TargetJob;
            var culture = top.Culture;

            if (top.ParentJob is { } parentJob)
            {
                if (jobId == Guid.Empty)
                {
                    throw new DataCorruptionException(top, $"{nameof(top.ParentJob)} is non-null on a data for default job.");
                }
                else if (parentJob == jobId)
                {
                    throw new DataCorruptionException(top, $"{nameof(top.ParentJob)} is the same as {nameof(top.TargetJob)}.");
                }

                if (loaded.Add((parentJob, culture)))
                {
                    // When fallback across job we will start on the same culture.
                    parents.AddRange(await this.repository.GetAsync(userId, parentJob, culture, type, cancellationToken));
                }
            }
            else if (!culture.Equals(CultureInfo.InvariantCulture))
            {
                if (loaded.Add((jobId, culture.Parent)))
                {
                    parents.AddRange(await this.repository.GetAsync(userId, jobId, culture.Parent, type, cancellationToken));
                }
            }
        }

        return parents;
    }
}
