namespace Cloudsume.Controllers;

using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Cloudsume.Activities;
using Cloudsume.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NetCaptcha;
using AuthenticationSchemes = Cloudsume.Identity.AuthenticationSchemes;
using Domain = Cloudsume.Identity.GuestSession;
using IGuestSessionRepository = Cloudsume.Identity.IGuestSessionRepository;
using IResumeLinkRepository = Cloudsume.Resume.IResumeLinkRepository;
using IResumeRepository = Candidate.Server.Resume.IResumeRepository;
using IStatsRepository = Cloudsume.Analytics.IStatsRepository;
using ITemplateLicenseRepository = Cloudsume.Resume.ITemplateLicenseRepository;
using IUserActivityRepository = Cloudsume.Analytics.IUserActivityRepository;
using IUserRepository = Cloudsume.Identity.IUserRepository;

[ApiController]
[Route("guest-sessions")]
public sealed class GuestSessionsController : ControllerBase
{
    private readonly JwtBearerOptions jwt;
    private readonly IGuestSessionRepository repository;
    private readonly IResumeRepository resumes;
    private readonly IResumeLinkRepository links;
    private readonly ITemplateLicenseRepository licenses;
    private readonly IUserRepository users;
    private readonly IAuthorizationService authorization;
    private readonly ICaptchaService captcha;
    private readonly IUserActivityRepository activities;
    private readonly IStatsRepository stats;
    private readonly ILogger logger;

    public GuestSessionsController(
        IOptionsSnapshot<JwtBearerOptions> jwt,
        IGuestSessionRepository repository,
        IResumeRepository resumes,
        IResumeLinkRepository links,
        ITemplateLicenseRepository licenses,
        IUserRepository users,
        IAuthorizationService authorization,
        ICaptchaService captcha,
        IUserActivityRepository activities,
        IStatsRepository stats,
        ILogger<GuestSessionsController> logger)
    {
        this.jwt = jwt.Get(AuthenticationSchemes.Guest);
        this.repository = repository;
        this.resumes = resumes;
        this.links = links;
        this.licenses = licenses;
        this.users = users;
        this.authorization = authorization;
        this.captcha = captcha;
        this.activities = activities;
        this.stats = stats;
        this.logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] CreateGuestSession body, CancellationToken cancellationToken = default)
    {
        var requester = this.HttpContext.Connection.RemoteIpAddress;

        if (requester == null)
        {
            // FIXME: Bad Request is not really correct.
            return this.BadRequest();
        }

        // Validate CAPTCHA.
        var captchaResult = await this.captcha.ValidateReCaptchaAsync(
            body.Captcha,
            "guest_session_create",
            requester,
            null,
            cancellationToken);

        if (!captchaResult)
        {
            this.ModelState.TryAddModelError(nameof(body.Captcha), "Invalid CAPTCHA challenge.");
            return this.BadRequest(this.ModelState);
        }

        // Create session.
        var issuer = this.jwt.TokenValidationParameters.ValidIssuers.First();
        var key = this.jwt.TokenValidationParameters.IssuerSigningKeys.First();
        var domain = new Domain(Guid.NewGuid(), issuer, key.KeyId, requester, DateTime.Now, null, null);

        await this.repository.CreateAsync(domain, cancellationToken);

        // Write stats.
        try
        {
            await this.stats.WriteGuestSessionCreatedAsync();
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Failed to write guest session created stats.");
        }

        // Build JWT claims.
        var claims = new List<Claim>()
        {
            new(ClaimTypes.NameIdentifier, domain.UserId.ToString()),
        };

        // Serialize JWT.
        var signing = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(domain.Issuer, null, claims, domain.CreatedAt.ToUniversalTime(), null, signing);
        var jwt = new JwtSecurityTokenHandler().WriteToken(token);

        return this.Ok(jwt);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync([FromRoute] Guid id, [FromQuery(Name = "token")] string? jwt, CancellationToken cancellationToken = default)
    {
        if (!string.IsNullOrEmpty(jwt))
        {
            // Check if authenticated user is not a guest account.
            var user = this.User;

            if (user.Identity is not { } identity || !identity.IsAuthenticated)
            {
                return this.Forbid();
            }

            var issuer = user.Claims.First(c => c.Type == JwtRegisteredClaimNames.Iss);

            if (issuer.Value == "cloudsume")
            {
                return this.Forbid();
            }

            // Check if user have required permissions.
            var requiredPolicies = new[]
            {
                AuthorizationPolicies.ResumeWrite,
                AuthorizationPolicies.TemplateLicenseWrite,
            };

            foreach (var policy in requiredPolicies)
            {
                var auth = await this.authorization.AuthorizeAsync(user, policy);

                if (!auth.Succeeded)
                {
                    return this.Forbid();
                }
            }

            // Check token.
            ClaimsPrincipal? guest = null;

            foreach (var validator in this.jwt.SecurityTokenValidators)
            {
                if (!validator.CanReadToken(jwt))
                {
                    continue;
                }

                try
                {
                    guest = validator.ValidateToken(jwt, this.jwt.TokenValidationParameters, out var _);
                }
                catch (Exception)
                {
                    continue;
                }

                break;
            }

            if (guest == null || this.users.GetId(guest) != id)
            {
                return this.BadRequest();
            }

            // Check if geust account already transferred.
            var session = await this.repository.GetAsync(id, cancellationToken);

            if (session == null)
            {
                // TODO: Throw an exception instead.
                return this.BadRequest();
            }
            else if (session.TransferredTo != null)
            {
                return this.NotFound();
            }

            // Check if destination user can import data.
            var to = this.users.GetId(user);

            if (await this.resumes.CountAsync(to, cancellationToken) != 0 || (await this.licenses.ListByUserAsync(to, cancellationToken)).Any())
            {
                return this.Conflict();
            }

            // Transfer data.
            if (!await this.resumes.TransferDataAsync(id, to, cancellationToken))
            {
                return this.Conflict();
            }

            foreach (var resumeId in await this.resumes.TransferAsync(id, to))
            {
                await this.links.TransferAsync(resumeId, to);
            }

            await this.licenses.TransferAsync(id, to);

            // Update guest session.
            await this.repository.SetTransferredAsync(id, to, DateTime.Now);

            // Write the activity.
            await this.activities.WriteAsync(new DeleteGuestActivity(to, id, this.GetRemoteIp(), this.GetUserAgent()));

            return this.NoContent();
        }
        else
        {
            return this.Forbid();
        }
    }
}
