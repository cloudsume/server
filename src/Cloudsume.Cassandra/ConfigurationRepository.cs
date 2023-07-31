namespace Cloudsume.Cassandra;

using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Cloudsume.Configurations;

internal sealed class ConfigurationRepository : IConfigurationRepository
{
    private readonly IMapperFactory db;

    public ConfigurationRepository(IMapperFactory db)
    {
        this.db = db;
    }

    public async Task<Uri?> GetSlackUriAsync(CancellationToken cancellationToken = default)
    {
        var data = await this.ReadAsync("slack.uri");

        if (data == null)
        {
            return null;
        }

        return new(Encoding.UTF8.GetString(data));
    }

    public Task SetSlackUriAsync(Uri? uri, CancellationToken cancellationToken = default)
    {
        byte[]? value;

        if (uri == null)
        {
            value = null;
        }
        else
        {
            value = Encoding.UTF8.GetBytes(uri.AbsoluteUri);
        }

        return this.WriteAsync("slack.uri", value);
    }

    private Task WriteAsync(string name, byte[]? value)
    {
        var db = this.db.CreateMapper();

        return db.UpdateAsync<Models.Configuration>("SET value = ? WHERE name = ?", value, name);
    }

    private async Task<byte[]?> ReadAsync(string name)
    {
        var db = this.db.CreateMapper();
        var row = await db.FirstOrDefaultAsync<Models.Configuration>("WHERE name = ?", name);

        return row?.Value;
    }
}
