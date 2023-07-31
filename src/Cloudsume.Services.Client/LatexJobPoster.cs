namespace Cloudsume.Services.Client;

using System;
using System.IO;
using System.IO.Pipelines;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using TapeArchive;
using Ultima.Extensions.Http;
using Ultima.Extensions.Mime;

public sealed class LatexJobPoster : ILatexJobPoster
{
    private readonly HttpClient http; // Don't dispose this.
    private readonly HttpRequestMessage request;
    private readonly Task<HttpResponseMessage> response;
    private ArchiveBuilder? writer;
    private bool disposed;

    public LatexJobPoster(HttpClient http, Uri uri)
    {
        this.http = http;
        this.request = new HttpRequestMessage(HttpMethod.Post, uri);

        try
        {
            var pipe = new Pipe();

            this.writer = new(pipe.Writer.AsStream(), false);
            this.request.Content = new StreamContent(pipe.Reader.AsStream());
            this.response = this.http.SendAsync(this.request, HttpCompletionOption.ResponseHeadersRead);
        }
        catch
        {
            this.writer?.Dispose();
            this.request.Dispose();
            throw;
        }
    }

    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync()
    {
        await this.DisposeAsyncCore();
        this.Dispose(false);
        GC.SuppressFinalize(this);
    }

    public async Task WriteAssetAsync(string name, int size, Stream content, CancellationToken cancellationToken = default)
    {
        if (this.writer == null)
        {
            throw new InvalidOperationException("This job is already finished.");
        }

        var item = new UstarItem(PrePosixType.RegularFile, new("./" + name))
        {
            Size = size,
            Content = content,
        };

        await this.writer.WriteItemAsync(item, null, cancellationToken);
    }

    public async Task<Stream> FinishAsync(CancellationToken cancellationToken = default)
    {
        // Sanity checks.
        if (this.writer == null)
        {
            throw new InvalidOperationException("This job is already finished.");
        }

        // Complete the request.
        await this.writer.CompleteAsync(cancellationToken);
        await this.writer.DisposeAsync();

        this.writer = null;

        // Process response.
        var response = await this.response;

        try
        {
            return await this.ProcessResponseAsync(response, cancellationToken);
        }
        catch
        {
            response.Dispose();
            throw;
        }
    }

    private async Task<Stream> ProcessResponseAsync(HttpResponseMessage response, CancellationToken cancellationToken = default)
    {
        // Check status code.
        if (response.StatusCode != HttpStatusCode.OK)
        {
            throw new HttpRequestException($"Unexpected status code from server ({response.StatusCode}).", null, response.StatusCode);
        }

        // Check content type.
        var content = response.Content;
        var type = content.Headers.ContentType;

        if (type.IsType("text/x.command-output"))
        {
            throw new LatexJobException(await content.ReadAsStringAsync(cancellationToken));
        }
        else if (type.IsType("application/pdf"))
        {
            var body = await content.ReadAsStreamAsync(cancellationToken);

            try
            {
                return new ResponseStream(response, body);
            }
            catch
            {
                await body.DisposeAsync();
                throw;
            }
        }
        else
        {
            throw new HttpRequestException($"Unexpected content type '{type}'.");
        }
    }

    private void Dispose(bool disposing)
    {
        if (this.disposed)
        {
            return;
        }

        if (disposing)
        {
            if (this.writer != null)
            {
                // Canel the request.
                this.writer.Dispose();

                try
                {
                    this.response.Result.Dispose();
                }
                catch
                {
                    // Ignore.
                }
            }

            this.request.Dispose();
        }

        this.disposed = true;
    }

    private async ValueTask DisposeAsyncCore()
    {
        if (this.disposed)
        {
            return;
        }

        if (this.writer != null)
        {
            // Cancel the request.
            await this.writer.DisposeAsync();

            try
            {
                (await this.response).Dispose();
            }
            catch
            {
                // Ignore.
            }
        }

        this.request.Dispose();
    }
}
