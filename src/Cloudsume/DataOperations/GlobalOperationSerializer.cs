namespace Cloudsume.DataOperations;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using IDataManagerCollection = Cloudsume.Resume.IDataActionCollection<Cloudsume.IDataManager>;

internal sealed class GlobalOperationSerializer : OperationSerializer, IGlobalOperationSerializer
{
    public GlobalOperationSerializer(IDataManagerCollection managers, IActionContextAccessor contextAccessor, IObjectModelValidator validator)
        : base(managers, contextAccessor, validator)
    {
    }

    public async Task<IEnumerable<DataOperation>?> DeserializeAsync(IFormCollection request, CancellationToken cancellationToken = default)
    {
        // Parse the request.
        ParsedRequest<Guid> parsed;

        try
        {
            parsed = this.ParseRequest(request, s =>
            {
                var id = Guid.Parse(s);

                if (id == Guid.Empty)
                {
                    throw new FormatException("The value is an empty GUID.");
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

            if (key.Id is { } id)
            {
                operation = new DeleteMultiplicableGlobalData(key, key.Type, id);
            }
            else
            {
                operation = new DeleteGlobalData(key, key.Type);
            }

            operations.Add(operation);
        }

        // Create update operations.
        var contents = parsed.Contents;
        var completes = new Dictionary<string, object>();

        foreach (var (key, value) in parsed.Updates)
        {
            // Do not allow index to be specified.
            if (key.Index is not null)
            {
                this.ModelState.AddModelError(key, "Unknow data type.");
                return null;
            }

            // Deserialize data.
            var data = await this.DeserializeAsync(key, value, (manager, key, data) =>
            {
                return this.DeserializeUpdateAsync(manager, key, data, contents, cancellationToken);
            });

            if (data is null)
            {
                return null;
            }

            // Create the operation.
            if (data is Candidate.Server.Resume.MultiplicativeData m)
            {
                if (m.Id is not { } id)
                {
                    // We don't support adding aggregated data.
                    this.ModelState.AddModelError(key, "Invalid identifier.");
                    return null;
                }
                else if (id != Guid.Empty)
                {
                    // If ID is not Empty that mean it is an update to the existing data.
                    if (!completes.TryGetValue(m.Type, out var set))
                    {
                        completes.Add(m.Type, set = new HashSet<Guid>());
                    }

                    if (!((HashSet<Guid>)set).Add(id))
                    {
                        this.ModelState.AddModelError(key, "Duplicated update.");
                        return null;
                    }
                }
            }
            else if (!completes.TryAdd(data.Type, new()))
            {
                this.ModelState.AddModelError(key, "Duplicated update.");
                return null;
            }

            operations.Add(new UpdateGlobalData(key, data));
        }

        return operations;
    }
}
