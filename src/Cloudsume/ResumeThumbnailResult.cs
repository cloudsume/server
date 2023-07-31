namespace Candidate.Server
{
    using System;
    using System.Threading.Tasks;
    using Cloudsume.Resume;
    using Microsoft.AspNetCore.Mvc;
    using Ultima.Extensions.Graphics;

    public sealed class ResumeThumbnailResult : ActionResult
    {
        public ResumeThumbnailResult(Thumbnail thumbnail)
        {
            this.Thumbnail = thumbnail;
        }

        public Thumbnail Thumbnail { get; }

        public override async Task ExecuteResultAsync(ActionContext context)
        {
            try
            {
                var response = context.HttpContext.Response;
                var content = this.Thumbnail.Content;

                response.ContentType = content.Format.GetContentType();

                await content.Data.CopyToAsync(response.Body, context.HttpContext.RequestAborted);
            }
            catch (OperationCanceledException)
            {
                context.HttpContext.Abort();
            }
            finally
            {
                await this.Thumbnail.DisposeAsync();
            }
        }
    }
}
