namespace Cloudsume.DataOperations;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using IDataManagerCollection = Cloudsume.Resume.IDataActionCollection<Cloudsume.IDataManager>;

internal sealed class DataOperationSerializer : OperationSerializer, IDataOperationSerializer
{
    public DataOperationSerializer(IDataManagerCollection managers, IActionContextAccessor contextAccessor, IObjectModelValidator validator)
        : base(managers, contextAccessor, validator)
    {
    }

    public async Task<IEnumerable<DataOperation>?> DeserializeAsync(IFormCollection request, CancellationToken cancellationToken = default)
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

        // Create delete operations in reverse order so the highest position will get delete first.
        var operations = new List<DataOperation>();

        foreach (var key in parsed.Deletes.Reverse())
        {
            DataOperation operation;

            if (key.Id is { } id)
            {
                operation = new DeleteMultiplicableLocalData(key, key.Type, id);
            }
            else
            {
                operation = new DeleteLocalData(key, key.Type);
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

        // Deserialize the operations.
        var contents = parsed.Contents;

        foreach (var (key, value) in updates)
        {
            // Deserialize the operation.
            var data = await this.DeserializeAsync(key, value, (manager, key, data) =>
            {
                return this.DeserializeUpdateAsync(manager, key, data, contents, cancellationToken);
            });

            if (data is null)
            {
                return null;
            }

            // Create update operation.
            DataOperation operation;

            if (data is Candidate.Server.Resume.MultiplicativeData m)
            {
                if (key.Index is not { } index)
                {
                    this.ModelState.AddModelError(key, "No index is specified.");
                    return null;
                }

                if (m.Id is not { } id || id != Guid.Empty)
                {
                    this.ModelState.AddModelError(key, "Invalid identifier.");
                    return null;
                }

                operation = new UpdateMultiplicableLocalData(key, m, index);
            }
            else
            {
                if (key.Index is not null)
                {
                    this.ModelState.AddModelError(key, "Unknow data type.");
                    return null;
                }

                operation = new UpdateLocalData(key, data);
            }

            operations.Add(operation);
        }

        return operations;
    }
}
