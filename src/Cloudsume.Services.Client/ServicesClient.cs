namespace Cloudsume.Services.Client;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using Microsoft.Extensions.Options;
using TapeArchive;
using Ultima.Extensions.Graphics;
using Ultima.Extensions.IO;
using Ultima.Extensions.Mime;
using UriBuilder = Ultima.Extensions.Primitives.UriBuilder;

public sealed class ServicesClient : IServicesClient
{
    private readonly ServicesClientOptions options;
    private readonly HttpClient http;

    public ServicesClient(IOptions<ServicesClientOptions> options, HttpClient http)
    {
        this.options = options.Value;
        this.http = http;
    }

    public ILatexJobPoster NewLatexJob()
    {
        return new LatexJobPoster(this.http, new(this.options.ServerUri, "latex/jobs"));
    }

    public async IAsyncEnumerable<ImageData> RenderPdfAsync(Stream pdf, int? size, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        // Create URL.
        var url = UriBuilder.For(this.options.ServerUri).AppendPath("pdf/jobs/render");

        if (size.HasValue)
        {
            url.AppendQuery("size", size.Value.ToString(CultureInfo.InvariantCulture));
        }

        // Set up a request.
        using var request = new HttpRequestMessage(HttpMethod.Post, url.BuildUri());
        await using var input = new StreamProtector(pdf);
        using var content = new StreamContent(input);

        request.Content = content;

        // Submit the request.
        using var response = await this.http.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

        if (response.StatusCode != HttpStatusCode.OK)
        {
            throw new HttpRequestException("Unexpected status code from server.", null, response.StatusCode);
        }

        // Read body.
        var body = response.Content;
        var type = body.Headers.ContentType;

        if (type.IsType("text/x.command-output"))
        {
            throw new PdfRenderException(await content.ReadAsStringAsync(cancellationToken));
        }
        else if (type.IsType("application/x-tar"))
        {
            await using var tar = await body.ReadAsStreamAsync(cancellationToken);
            await using var reader = new TapeArchive(tar, true);

            await foreach (var file in reader.ReadAsync(cancellationToken))
            {
                // Skip the root directory './'.
                if (file.IsDirectory)
                {
                    continue;
                }

                yield return new(ImageFormat.JPEG, file.Content, Convert.ToInt32(file.Size), true);
            }
        }
        else
        {
            throw new HttpRequestException($"Unexpected content type '{type}'.");
        }
    }
}
