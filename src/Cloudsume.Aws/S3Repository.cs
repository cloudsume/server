namespace Cloudsume.Aws;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Candidate.Server.Aws;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

public abstract class S3Repository : IDisposable
{
    protected const string KeyDelimiter = "/";

    private readonly S3RepositoryOptions options;
    private readonly ILogger logger;
    private bool disposed;

    protected S3Repository(IOptions<S3RepositoryOptions> options, ILogger<S3Repository> logger)
    {
        this.options = options.Value;
        this.Client = new AmazonS3Client();
        this.logger = logger;
    }

    protected AmazonS3Client Client { get; }

    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected static bool IsValidName(string name)
    {
        if (name == "." || name == ".." || name == "~" || name == KeyDelimiter)
        {
            return false;
        }

        return true;
    }

    protected static Key ParseKey(string key)
    {
        if (key.Length == 0)
        {
            throw new ArgumentException("The value is empty.", nameof(key));
        }

        // Extract directory and file name.
        var index = key.LastIndexOf(KeyDelimiter, StringComparison.Ordinal);
        string? directory;
        string file;

        if (index == -1)
        {
            directory = null;
            file = key;
        }
        else if (index == 0 || index == key.Length - KeyDelimiter.Length)
        {
            // key is begin with / (e.g. /abc) or ended with / (e.g. abc/).
            throw new ArgumentException("The value is not a valid key.", nameof(key));
        }
        else
        {
            directory = key[..index];
            file = key[(index + 1)..];
        }

        if (!IsValidName(file))
        {
            throw new ArgumentException("The value has invalid file name.", nameof(key));
        }

        // Extract extension.
        index = file.LastIndexOf('.');

        if (index == -1)
        {
            return new(directory, file, null);
        }
        else if (index == 0)
        {
            // file is begin with . (e.g. .gitignore).
            return new(directory, null, file[1..]);
        }
        else if (index == file.Length - 1)
        {
            // file is ended with . (abc.).
            throw new ArgumentException("The value is not a valid file name.", nameof(key));
        }
        else
        {
            return new Key(directory, file[..index], file[(index + 1)..]);
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposed)
        {
            if (disposing)
            {
                this.Client.Dispose();
            }

            this.disposed = true;
        }
    }

    protected Task PutObjectAsync(string key, Stream content, long size, CancellationToken cancellationToken = default)
    {
        return this.PutObjectAsync(key, content, size, "application/octet-stream", cancellationToken);
    }

    protected async Task PutObjectAsync(string key, Stream content, long size, string contentType, CancellationToken cancellationToken = default)
    {
        for (; ;)
        {
            var request = new PutObjectRequest()
            {
                AutoCloseStream = false,
                AutoResetStreamPosition = false,
                BucketName = this.options.Bucket,
                ContentType = contentType,
                InputStream = content,
                Key = key,
            };

            request.Headers.ContentLength = size;

            try
            {
                await this.Client.PutObjectAsync(request, cancellationToken);
            }
            catch (AmazonS3Exception ex) when (ex.StatusCode == HttpStatusCode.ServiceUnavailable)
            {
                // https://github.com/ultimicro/cloudsume-server/issues/234
                await Task.Delay(1000, cancellationToken);
                continue;
            }

            break;
        }
    }

    protected async Task<Stream> GetObjectContentAsync(string key, CancellationToken cancellationToken = default)
    {
        var response = await this.Client.GetObjectAsync(this.options.Bucket, key, cancellationToken);

        try
        {
            return new S3ResponseStream(response);
        }
        catch
        {
            response.Dispose();
            throw;
        }
    }

    protected async Task<GetObjectResponse> GetObjectAsync(string key, CancellationToken cancellationToken = default)
    {
        var response = await this.Client.GetObjectAsync(this.options.Bucket, key, cancellationToken);

        try
        {
            this.logger.LogInformation("Loaded object '{Key}'.", response.Key);
        }
        catch
        {
            response.Dispose();
            throw;
        }

        return response;
    }

    protected async Task<IEnumerable<S3Object>> ListObjectsAsync(string prefix, CancellationToken cancellationToken = default)
    {
        var response = await this.Client.ListObjectsV2Async(
            new()
            {
                BucketName = this.options.Bucket,
                Prefix = prefix,
            },
            cancellationToken);

        // Filter out empty object that act as a directory.
        return response.S3Objects.Where(o => o.Key != prefix);
    }

    protected Task DeleteObjectAsync(string key, CancellationToken cancellationToken = default)
    {
        return this.Client.DeleteObjectAsync(this.options.Bucket, key, cancellationToken);
    }

    protected async Task DeleteObjectsAsync(IEnumerable<string> keys, CancellationToken cancellationToken = default)
    {
        if (!keys.Any())
        {
            return;
        }

        var deletion = await this.Client.DeleteObjectsAsync(
            new()
            {
                BucketName = this.options.Bucket,
                Objects = keys.Select(k => new KeyVersion() { Key = k }).ToList(),
                Quiet = true,
            },
            cancellationToken);

        if (deletion.DeleteErrors.Count != 0)
        {
            var ex = new IOException("Cannot delete some of S3 objects.");

            foreach (var error in deletion.DeleteErrors)
            {
                ex.Data.Add(error.Key, error.Code);
            }

            throw ex;
        }
    }

    protected readonly ref struct Key
    {
        public Key(string? directory, string? file, string? extension)
        {
            this.Directory = directory;
            this.File = file;
            this.Extension = extension;
        }

        public string? Directory { get; }

        /// <summary>
        /// Gets file name of this key.
        /// </summary>
        /// <value>
        /// The file name without extension or <c>null</c> if file have only extension (e.g. .gitignore).
        /// </value>
        public string? File { get; }

        public string? Extension { get; }

        public void Deconstruct(out string? directory, out string? file, out string? extension)
        {
            directory = this.Directory;
            file = this.File;
            extension = this.Extension;
        }
    }
}
