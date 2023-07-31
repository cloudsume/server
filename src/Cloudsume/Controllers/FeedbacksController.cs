namespace Cloudsume.Controllers;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudsume.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NetCaptcha;
using NetUlid;
using Ultima.Extensions.Security;
using Domain = Cloudsume.Analytics.Feedback;
using IFeedbackRepository = Cloudsume.Analytics.IFeedbackRepository;
using IStatsRepository = Cloudsume.Analytics.IStatsRepository;
using IUserRepository = Cloudsume.Identity.IUserRepository;

[ApiController]
[Route("feedbacks")]
public sealed class FeedbacksController : ControllerBase
{
    private readonly IUserRepository users;
    private readonly IFeedbackRepository repository;
    private readonly ICaptchaService captcha;
    private readonly IStatsRepository stats;
    private readonly ILogger logger;

    public FeedbacksController(
        IUserRepository users,
        IFeedbackRepository repository,
        ICaptchaService captcha,
        IStatsRepository stats,
        ILogger<FeedbacksController> logger)
    {
        this.users = users;
        this.repository = repository;
        this.captcha = captcha;
        this.stats = stats;
        this.logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] CreateFeedback request, CancellationToken cancellationToken = default)
    {
        // Check if the request is valid.
        var score = request.Score;
        var detail = request.Detail;

        if (score is null && detail is null)
        {
            return this.BadRequest();
        }

        // Get client address.
        var from = this.HttpContext.Connection.RemoteIpAddress;

        if (from is null)
        {
            throw new InvalidOperationException("No IP address of the client.");
        }

        // Validate CAPTCHA.
        var captchaResult = await this.captcha.ValidateReCaptchaAsync(
            request.CaptchaToken,
            "feedback_create",
            from,
            null,
            cancellationToken);

        if (!captchaResult)
        {
            this.ModelState.TryAddModelError(nameof(request.CaptchaToken), "Invalid CAPTCHA challenge.");
            return this.BadRequest(this.ModelState);
        }

        // Store feedback.
        Guid? userId = this.User.IsAuthenticated() ? this.users.GetId(this.User) : null;
        var userAgent = this.HttpContext.Request.Headers.UserAgent.FirstOrDefault() ?? string.Empty;
        var domain = new Domain(Ulid.Generate(), score, detail ?? string.Empty, request.Contact, userId, from, userAgent);

        await this.repository.CreateAsync(domain, cancellationToken);

        // Write stats.
        try
        {
            await this.stats.WriteFeedbackCreatedAsync();
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Failed to write feedback created stats.");
        }

        return this.NoContent();
    }

    [HttpGet]
    [Authorize(AuthorizationPolicies.FeedbackRead)]
    public async Task<IEnumerable<Feedback>> ListAsync(
        [FromQuery, Range(Domain.MinScore, Domain.MaxScore)] int? score,
        [FromQuery(Name = "skip_till")] Ulid? skipTill = null,
        CancellationToken cancellationToken = default)
    {
        var domains = await this.repository.ListAsync(score, skipTill, 100, cancellationToken);

        return domains.Select(d => new Feedback(d.Id, d.Detail, d.Contact)).ToArray();
    }
}
