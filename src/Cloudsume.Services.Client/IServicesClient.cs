namespace Cloudsume.Services.Client;

using System.Collections.Generic;
using System.IO;
using System.Threading;
using Ultima.Extensions.Graphics;

public interface IServicesClient
{
    ILatexJobPoster NewLatexJob();

    IAsyncEnumerable<ImageData> RenderPdfAsync(Stream pdf, int? size, CancellationToken cancellationToken = default);
}
