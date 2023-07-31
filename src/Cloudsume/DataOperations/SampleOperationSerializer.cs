namespace Cloudsume.DataOperations;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using IDataManagerCollection = Cloudsume.Resume.IDataActionCollection<Cloudsume.IDataManager>;

internal sealed class SampleOperationSerializer : OperationSerializer, ISampleOperationSerializer
{
    public SampleOperationSerializer(IDataManagerCollection managers, IActionContextAccessor contextAccessor, IObjectModelValidator validator)
        : base(managers, contextAccessor, validator)
    {
    }

    public async Task<IEnumerable<DataOperation>?> DeserializeAsync(
        Guid jobId,
        CultureInfo culture,
        IFormCollection request,
        CancellationToken cancellationToken = default)
    {
        // Parse the request.
        ParsedRequest<int> parsed;

        try
        {
            parsed = this.ParseRequest(request, s =>
            {
                var id = int.Parse(s, CultureInfo.InvariantCulture);

                if (id < 0)
                {
                    throw new FormatException("The value is negative.");
                }

                return id;
            });
        }
        catch (RequestException ex)
        {
            this.ModelState.AddModelError(ex.Key, ex.Message);
            return null;
        }

        // Create delete operations.
        var operations = new List<DataOperation>();

        foreach (var key in parsed.Deletes)
        {
            DataOperation operation;

            if (key.Id is { } index)
            {
                operation = new DeleteMultiplicableSampleData(key, key.Type, index);
            }
            else
            {
                operation = new DeleteSampleData(key, key.Type);
            }

            operations.Add(operation);
        }

        // Index the updates.
        var updates = new SortedDictionary<UpdateKey, object>();

        foreach (var update in parsed.Updates)
        {
            var key = update.Key;

            try
            {
                updates.Add(key, update.Value);
            }
            catch (ArgumentException)
            {
                this.ModelState.AddModelError(key, "Duplicated update.");
                return null;
            }
        }

        // Create the update operations.
        var contents = parsed.Contents;
        var targetIds = new HashSet<Guid>();

        foreach (var (key, value) in updates)
        {
            // Deserialize data.
            var data = await this.DeserializeAsync(key, value, async (manager, key, data) =>
            {
                // Deserialize data.
                ISampleUpdate update;

                try
                {
                    update = await manager.ReadSampleUpdateAsync(data, cancellationToken);
                }
                catch (DataUpdateException ex)
                {
                    this.ModelState.TryAddModelError(key, ex.Message);
                    return null;
                }

                // Check parent job.
                if (update.ParentJob is { } parentJob)
                {
                    if (jobId == Guid.Empty)
                    {
                        this.ModelState.TryAddModelError(key, "Default job cannot have parent job.");
                        return null;
                    }
                    else if (parentJob == jobId)
                    {
                        this.ModelState.TryAddModelError(key, "The parent job is the same as target job.");
                        return null;
                    }
                }

                // Convert the DTO to domain object.
                var domain = await this.ToDomainAsync(manager, key, update.Update, contents, cancellationToken);

                if (domain is null)
                {
                    return null;
                }

                return new Cloudsume.Resume.SampleData(jobId, culture, domain, update.ParentJob);
            });

            if (data is null)
            {
                return null;
            }

            // Create the operation.
            DataOperation operation;

            if (data.Data is Candidate.Server.Resume.MultiplicativeData m)
            {
                if (key.Index is not { } index)
                {
                    this.ModelState.AddModelError(key, "No index is specified.");
                    return null;
                }

                if (m.Id is not { } id)
                {
                    this.ModelState.AddModelError(key, "No data ID is specified.");
                    return null;
                }

                if (id != Guid.Empty && !targetIds.Add(id))
                {
                    this.ModelState.AddModelError(key, "Duplicated update.");
                    return null;
                }

                operation = new UpdateMultiplicableSampleData(key, data, index);
            }
            else
            {
                if (key.Index is not null)
                {
                    this.ModelState.AddModelError(key, "Unknow data type.");
                    return null;
                }

                operation = new UpdateSampleData(key, data);
            }

            operations.Add(operation);
        }

        return operations;
    }
}
