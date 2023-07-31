namespace Cloudsume.Aws;

using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Amazon.S3;
using Cloudsume.Resume;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

internal sealed class SamplePhotoRepository : S3Repository, ISamplePhotoRepository
{
    public SamplePhotoRepository(IOptions<SamplePhotoRepositoryOptions> options, ILogger<SamplePhotoRepository> logger)
        : base(options, logger)
    {
    }

    public Task DeleteAsync(Guid userId, Guid jobId, CultureInfo culture, CancellationToken cancellationToken = default)
    {
        var key = this.GetKey(userId, jobId, culture);

        return this.DeleteObjectAsync(key, cancellationToken);
    }

    public async Task<Stream?> ReadAsync(Guid userId, Guid jobId, CultureInfo culture, CancellationToken cancellationToken = default)
    {
        var key = this.GetKey(userId, jobId, culture);

        try
        {
            return await this.GetObjectContentAsync(key, cancellationToken);
        }
        catch (AmazonS3Exception ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public Task WriteAsync(Guid userId, Guid jobId, CultureInfo culture, Stream photo, int size, CancellationToken cancellationToken = default)
    {
        return this.PutObjectAsync(this.GetKey(userId, jobId, culture), photo, size, cancellationToken);
    }

    private string GetKey(Guid userId, Guid jobId, CultureInfo culture)
    {
        return $"{userId}{KeyDelimiter}{jobId}{KeyDelimiter}{this.GetFileName(culture)}";
    }

    private string GetFileName(CultureInfo culture)
    {
        if (culture.Equals(CultureInfo.InvariantCulture))
        {
            return "default";
        }
        else
        {
            return culture.Name;
        }
    }
}
