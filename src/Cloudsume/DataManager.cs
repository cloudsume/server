namespace Cloudsume;

using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

internal abstract class DataManager
{
    private readonly JsonOptions json;

    protected DataManager(IOptions<JsonOptions> json)
    {
        this.json = json.Value;
    }

    protected async Task<T> ReadJsonAsync<T>(Stream data, CancellationToken cancellationToken = default)
    {
        var options = this.json.JsonSerializerOptions;
        T? dto;

        try
        {
            dto = await JsonSerializer.DeserializeAsync<T>(data, options, cancellationToken);
        }
        catch (JsonException ex)
        {
            throw new DataUpdateException("Invalid data.", ex);
        }

        return dto ?? throw new DataUpdateException("Invalid data.");
    }
}
