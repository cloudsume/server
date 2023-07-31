namespace Cloudsume.Cassandra;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Candidate.Server.Resume;
using Candidate.Server.Resume.Data;
using Cloudsume.Resume;
using Cornot;
using global::Cassandra;
using global::Cassandra.Mapping;

internal sealed class SampleDataRepository : ISampleDataRepository
{
    private readonly IMapperFactory db;
    private readonly IReadConsistencyProvider readConsistencies;
    private readonly IDataActionCollection<IResumeDataMapper> mappers;
    private readonly IDataActionCollection<ISampleDataPayloadManager> payloads;
    private readonly IReadOnlyDictionary<string, SampleDataManager> managers;

    public SampleDataRepository(
        IMapperFactory db,
        IReadConsistencyProvider readConsistencies,
        IDataActionCollection<IResumeDataMapper> mappers,
        IDataActionCollection<ISampleDataPayloadManager> payloads)
    {
        this.db = db;
        this.readConsistencies = readConsistencies;
        this.mappers = mappers;
        this.payloads = payloads;
        this.managers = SampleDataManager.BuildTable(db.CreateMapper().GetType());
    }

    public async Task DeleteAsync(Guid userId, Guid jobId, CultureInfo culture, string type, Guid? id, CancellationToken cancellationToken = default)
    {
        // Load a manager for target type.
        if (!this.managers.TryGetValue(type, out var manager))
        {
            throw new ArgumentException($"Unknow data type '{type}'.", nameof(type));
        }

        // Setup a DTO for target row.
        var target = manager.NewDto();

        target.Owner = userId;
        target.TargetJob = jobId;
        target.Culture = culture.Name;

        if (target is Models.IMultiplicableSampleData m)
        {
            if (id is null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            m.Id = id.Value;
        }
        else if (id is not null)
        {
            throw new ArgumentException("The value must be null for a unique data.", nameof(id));
        }

        // Execute row deletion.
        var db = this.db.CreateMapper();

        await manager.DeleteAsync(db, target, CqlQueryOptions.New().SetConsistencyLevel(ConsistencyLevel.EachQuorum));

        // Delete row payload if available.
        if (this.payloads.TryGetValue(type, out var payload))
        {
            await payload.DeletePayloadAsync(target);
        }
    }

    public async Task<IEnumerable<SampleData>> GetAsync(
        Guid userId,
        Guid jobId,
        CultureInfo culture,
        string type,
        CancellationToken cancellationToken = default)
    {
        // Load manager.
        if (!this.managers.TryGetValue(type, out var manager))
        {
            throw new ArgumentException($"Unknow data type '{type}'.", nameof(type));
        }

        // Fetch data.
        var db = this.db.CreateMapper();
        var rows = await manager.GetAsync(db, userId, jobId, culture, options => options.SetConsistencyLevel(this.readConsistencies.StrongConsistency));
        var result = new List<SampleData>();

        foreach (var row in rows)
        {
            var domain = await this.ToDomainAsync(row, cancellationToken);

            result.Add(new(row.TargetJob, CultureInfo.GetCultureInfo(row.Culture), domain, row.ParentJob));
        }

        return result;
    }

    public async Task<SampleData<Photo>?> GetPhotoAsync(Guid userId, Guid jobId, CultureInfo culture, CancellationToken cancellationToken = default)
    {
        var db = this.db.CreateMapper();
        var manager = this.managers[Photo.StaticType];
        var rows = await manager.GetAsync(db, userId, jobId, culture, options => options.SetConsistencyLevel(this.readConsistencies.StrongConsistency));
        var row = rows.SingleOrDefault();

        if (row is null)
        {
            return null;
        }

        var domain = (Photo)await this.ToDomainAsync(row, cancellationToken);

        return new(row.TargetJob, CultureInfo.GetCultureInfo(row.Culture), domain, row.ParentJob);
    }

    public async Task<IEnumerable<SampleData>> ListAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var db = this.db.CreateMapper();
        var domains = new List<SampleData>();

        foreach (var (_, manager) in this.managers)
        {
            var rows = await manager.GetAsync(db, userId, null, null, options => options.SetConsistencyLevel(this.readConsistencies.StrongConsistency));

            foreach (var row in rows)
            {
                var domain = await this.ToDomainAsync(row, cancellationToken);

                domains.Add(new(row.TargetJob, CultureInfo.GetCultureInfo(row.Culture), domain, row.ParentJob));
            }
        }

        return domains;
    }

    public async Task WriteAsync(Guid userId, SampleData data, int? position, CancellationToken cancellationToken = default)
    {
        // Setup a DTO.
        var type = data.Data.Type;
        var manager = this.managers[type];
        var row = manager.NewDto();

        row.Owner = userId;
        row.TargetJob = data.TargetJob;
        row.Culture = data.Culture.Name;
        row.Data = this.mappers[type].ToCassandra(data.Data);
        row.ParentJob = data.ParentJob;

        if (row is Models.IMultiplicableSampleData m)
        {
            // Get ID.
            if (((MultiplicativeData)data.Data).Id is not { } id)
            {
                throw new ArgumentException("No data identifier is specified.", nameof(data));
            }
            else if (id == Guid.Empty)
            {
                throw new ArgumentException("The value is local data.", nameof(data));
            }

            // Get position.
            if (position is null)
            {
                throw new ArgumentNullException(nameof(position));
            }
            else if (position < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(position));
            }

            m.Id = id;
            m.Position = Convert.ToSByte(position.Value);
            m.Parent = ((MultiplicativeData)data.Data).BaseId;
        }
        else if (position is not null)
        {
            throw new ArgumentException("The value must be null for unique data.", nameof(position));
        }

        // Write the row.
        var db = this.db.CreateMapper();

        await manager.UpdateAsync(db, row, CqlQueryOptions.New().SetConsistencyLevel(ConsistencyLevel.EachQuorum));

        // Write the payload.
        if (this.payloads.TryGetValue(type, out var payload))
        {
            await payload.WritePayloadAsync(data.Data, row);
        }
    }

    private async ValueTask<ResumeData> ToDomainAsync(Models.IResumeSampleData row, CancellationToken cancellationToken = default)
    {
        var data = row.Data;

        if (data == null)
        {
            throw new DataCorruptionException(row, $"{nameof(row.Data)} is null.");
        }

        var mapper = this.mappers[Models.DataObject.DomainTypes[data.GetType()]];
        var domain = row is Models.IMultiplicableSampleData m ? mapper.ToDomain(m.Id, m.Parent, data) : mapper.ToDomain(Guid.Empty, null, data);

        await this.ReadPayloadAsync(domain, row, cancellationToken);

        return domain;
    }

    private async ValueTask ReadPayloadAsync(ResumeData domain, Models.IResumeSampleData row, CancellationToken cancellationToken = default)
    {
        if (this.payloads.TryGetValue(domain.Type, out var payload))
        {
            await payload.ReadPayloadAsync(domain, row, cancellationToken);
        }
    }
}
