namespace Cloudsume.Resume;

using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

internal sealed class FileSystemSamplePhotoRepository : ISamplePhotoRepository
{
    private readonly FileSystemSamplePhotoRepositoryOptions options;

    public FileSystemSamplePhotoRepository(IOptions<FileSystemSamplePhotoRepositoryOptions> options)
    {
        this.options = options.Value;
    }

    public Task DeleteAsync(Guid userId, Guid jobId, CultureInfo culture, CancellationToken cancellationToken = default)
    {
        var path = this.GetPath(userId, jobId, culture);

        try
        {
            File.Delete(path);
        }
        catch (DirectoryNotFoundException)
        {
            // Ignore.
        }

        return Task.CompletedTask;
    }

    public Task<Stream?> ReadAsync(Guid userId, Guid jobId, CultureInfo culture, CancellationToken cancellationToken = default)
    {
        var path = this.GetPath(userId, jobId, culture);
        Stream? photo;

        try
        {
            photo = File.OpenRead(path);
        }
        catch (Exception ex) when (ex is DirectoryNotFoundException || ex is FileNotFoundException)
        {
            photo = null;
        }

        try
        {
            return Task.FromResult(photo);
        }
        catch
        {
            photo?.Dispose();
            throw;
        }
    }

    public async Task WriteAsync(Guid userId, Guid jobId, CultureInfo culture, Stream photo, int size, CancellationToken cancellationToken = default)
    {
        await using var file = File.Create(this.GetPath(userId, jobId, culture));
        await photo.CopyToAsync(file);
    }

    private string GetPath(Guid userId, Guid jobId, CultureInfo culture)
    {
        return Path.Join(this.options.Path, userId.ToString(), jobId.ToString(), culture.Equals(CultureInfo.InvariantCulture) ? "default" : culture.Name);
    }
}
