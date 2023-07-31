namespace Cloudsume.Controllers
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Cloudsume.Models;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using ITemplateLicenseRepository = Cloudsume.Resume.ITemplateLicenseRepository;
    using IUserRepository = Cloudsume.Identity.IUserRepository;

    [ApiController]
    [Route("template-licenses")]
    public sealed class TemplateLicenseController : ControllerBase
    {
        private readonly ITemplateLicenseRepository repository;
        private readonly IUserRepository users;

        public TemplateLicenseController(ITemplateLicenseRepository repository, IUserRepository users)
        {
            this.repository = repository;
            this.users = users;
        }

        [HttpGet]
        [Authorize(AuthorizationPolicies.TemplateLicenseRead)]
        public async Task<IActionResult> GetAsync([FromQuery(Name = "template")] Guid? registrationId, CancellationToken cancellationToken = default)
        {
            var userId = this.users.GetId(this.User);

            if (registrationId != null)
            {
                var license = await this.repository.GetAsync(userId, registrationId.Value, cancellationToken);

                if (license == null)
                {
                    return this.NoContent();
                }

                return this.Ok(new TemplateLicense(license));
            }
            else
            {
                var licenses = await this.repository.ListByUserAsync(userId, cancellationToken);
                var response = licenses.Select(l => new TemplateLicense(l)).ToArray();

                return this.Ok(response);
            }
        }
    }
}
