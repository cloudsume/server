namespace Cloudsume.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Candidate.Server;
    using Cloudsume.Identity;
    using Cloudsume.Models;
    using Cornot;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using NetUlid;
    using Ultima.Extensions.DataValidation;
    using Domain = Cloudsume.Resume.Template;
    using ITemplatePreviewRepository = Cloudsume.Resume.ITemplatePreviewRepository;
    using ITemplateRepository = Cloudsume.Resume.ITemplateRepository;
    using RegistrationCategory = Cloudsume.Resume.RegistrationCategory;

    [Route("templates")]
    [ApiController]
    public sealed class TemplateController : ControllerBase
    {
        private readonly ITemplateRepository repository;
        private readonly ITemplatePreviewRepository previews;
        private readonly IUserRepository users;
        private readonly IAuthorizationService authorization;

        public TemplateController(
            ITemplateRepository repository,
            ITemplatePreviewRepository previews,
            IUserRepository users,
            IAuthorizationService authorization)
        {
            this.repository = repository;
            this.previews = previews;
            this.users = users;
            this.authorization = authorization;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsync([FromRoute] Ulid id, CancellationToken cancellationToken = default)
        {
            var template = await this.GetTemplateAsync(id, cancellationToken);

            if (template == null)
            {
                return this.NotFound();
            }

            var pages = await this.previews.GetPageCountAsync(id, cancellationToken);
            var previews = new List<string>();

            for (var i = 0; i < pages; i++)
            {
                var url = this.Url.ActionLink("GetPreview", values: new { id, page = i });

                if (url == null)
                {
                    throw new Exception($"Failed to get a URL to {nameof(this.GetPreviewAsync)}.");
                }

                previews.Add(url);
            }

            return this.Ok(new Template(template, previews));
        }

        [HttpGet("{id}/previews/{page}")]
        public async Task<IActionResult> GetPreviewAsync([FromRoute] Ulid id, [FromRoute, NonNegative] int page, CancellationToken cancellationToken = default)
        {
            var template = await this.GetTemplateAsync(id, cancellationToken);

            if (template == null)
            {
                return this.NotFound();
            }

            var preview = await this.previews.GetAsync(id, page, cancellationToken);

            if (preview == null)
            {
                throw new DataCorruptionException(template, "Template has no previews.");
            }

            try
            {
                return new ResumeThumbnailResult(preview);
            }
            catch
            {
                await preview.DisposeAsync();
                throw;
            }
        }

        private async Task<Domain?> GetTemplateAsync(Ulid id, CancellationToken cancellationToken = default)
        {
            var template = await this.repository.GetTemplateAsync(id, cancellationToken);

            if (template == null)
            {
                return null;
            }
            else if (template.Category == RegistrationCategory.Private)
            {
                var auth = await this.authorization.AuthorizeAsync(this.User, AuthorizationPolicies.TemplateRead);

                if (!auth.Succeeded)
                {
                    return null;
                }

                var registration = await this.repository.GetRegistrationAsync(template.RegistrationId, cancellationToken);

                if (registration == null)
                {
                    throw new DataCorruptionException(template, "Unknown registration.");
                }
                else if (registration.UserId != this.users.GetId(this.User))
                {
                    return null;
                }
            }

            return template;
        }
    }
}
