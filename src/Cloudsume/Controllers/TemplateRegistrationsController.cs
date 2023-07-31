namespace Cloudsume.Controllers;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Candidate.Server;
using Cloudsume.Activities;
using Cloudsume.Identity;
using Cloudsume.Models;
using Cloudsume.Options;
using Cornot;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using NetCaptcha;
using NetUlid;
using Ultima.Extensions.Collections;
using Ultima.Extensions.Currency;
using Ultima.Extensions.DataValidation;
using AssetFile = Cloudsume.Resume.AssetFile;
using AssetName = Cloudsume.Resume.AssetName;
using CancelPurchaseReason = Cloudsume.Template.CancelPurchaseReason;
using CompileException = Cloudsume.Resume.CompileException;
using CompileResult = Cloudsume.Resume.CompileResult;
using Domain = Cloudsume.Resume.TemplateRegistration;
using ICancelPurchaseFeedbackRepository = Cloudsume.Template.ICancelPurchaseFeedbackRepository;
using IDataAggregator = Cloudsume.Resume.IDataAggregator;
using IDataManagerCollection = Cloudsume.Resume.IDataActionCollection<Cloudsume.IDataManager>;
using IJobRepository = Cloudsume.Data.IJobRepository;
using IReceivingMethodRepository = Cloudsume.Financial.IReceivingMethodRepository;
using IReceivingMethodService = Cloudsume.Financial.IReceivingMethodService;
using IResumeCompiler = Cloudsume.Resume.IResumeCompiler;
using ISampleDataLoader = Cloudsume.Resume.ISampleDataLoader;
using ITemplateAssetRepository = Cloudsume.Resume.ITemplateAssetRepository;
using ITemplateLicenseRepository = Cloudsume.Resume.ITemplateLicenseRepository;
using ITemplatePackRepository = Cloudsume.Template.ITemplatePackRepository;
using ITemplatePreviewRepository = Cloudsume.Resume.ITemplatePreviewRepository;
using ITemplateRepository = Cloudsume.Resume.ITemplateRepository;
using ITemplateWorkspaceRepository = Cloudsume.Resume.ITemplateWorkspaceRepository;
using IThumbnailGenerator = Cloudsume.Resume.IThumbnailGenerator;
using IUserActivityRepository = Cloudsume.Analytics.IUserActivityRepository;
using IWorkspaceAssetRepository = Cloudsume.Resume.IWorkspaceAssetRepository;
using IWorkspacePreviewRepository = Cloudsume.Resume.IWorkspacePreviewRepository;
using PaymentCancelReason = Cloudsume.Financial.PaymentCancelReason;
using PaymentMethodStatus = Cloudsume.Financial.PaymentMethodStatus;
using ReceivingMethodStatus = Cloudsume.Financial.ReceivingMethodStatus;
using RegistrationCategory = Cloudsume.Resume.RegistrationCategory;
using TemplateException = Cloudsume.Resume.TemplateException;
using TemplateLicenseStatus = Cloudsume.Template.TemplateLicenseStatus;
using TemplateSyntaxException = Cloudsume.Resume.TemplateSyntaxException;

[ApiController]
[Route("template-registrations")]
public sealed class TemplateRegistrationsController : ControllerBase
{
    private const int MaxWorkspace = 1024 * 1024 * 5; // 5 MB.

    private readonly ApplicationOptions options;
    private readonly ITemplateRepository repository;
    private readonly ITemplateLicenseRepository licenses;
    private readonly ITemplatePackRepository packs;
    private readonly ITemplateWorkspaceRepository workspaces;
    private readonly IWorkspaceAssetRepository workspaceAssets;
    private readonly IDataManagerCollection managers;
    private readonly ISampleDataLoader sampleData;
    private readonly IDataAggregator aggregator;
    private readonly IResumeCompiler compiler;
    private readonly IThumbnailGenerator thumbnail;
    private readonly IWorkspacePreviewRepository previews;
    private readonly ITemplateAssetRepository templateAssets;
    private readonly ITemplatePreviewRepository templatePreviews;
    private readonly IUserRepository users;
    private readonly IJobRepository jobs;
    private readonly IReceivingMethodRepository receivingMethods;
    private readonly IReceivingMethodService receivingMethod;
    private readonly TemplatePriceConstraintOptions priceConstraints;
    private readonly ICancelPurchaseFeedbackRepository cancelPurchaseFeedbacks;
    private readonly IAuthorizationService authorization;
    private readonly ICaptchaService captcha;
    private readonly IUserActivityRepository activities;

    public TemplateRegistrationsController(
        IOptions<ApplicationOptions> options,
        ITemplateRepository repository,
        ITemplateLicenseRepository licenses,
        ITemplatePackRepository packs,
        ITemplateWorkspaceRepository workspaces,
        IWorkspaceAssetRepository workspaceAssets,
        IDataManagerCollection managers,
        ISampleDataLoader sampleData,
        IDataAggregator aggregator,
        IResumeCompiler compiler,
        IThumbnailGenerator thumbnail,
        IWorkspacePreviewRepository previews,
        ITemplateAssetRepository templateAssets,
        ITemplatePreviewRepository templatePreviews,
        IUserRepository users,
        IJobRepository jobs,
        IReceivingMethodRepository receivingMethods,
        IReceivingMethodService receivingMethod,
        IOptions<TemplatePriceConstraintOptions> priceConstraints,
        ICancelPurchaseFeedbackRepository cancelPurchaseFeedbacks,
        IAuthorizationService authorization,
        ICaptchaService captcha,
        IUserActivityRepository activities)
    {
        this.options = options.Value;
        this.repository = repository;
        this.licenses = licenses;
        this.packs = packs;
        this.workspaces = workspaces;
        this.workspaceAssets = workspaceAssets;
        this.managers = managers;
        this.sampleData = sampleData;
        this.aggregator = aggregator;
        this.compiler = compiler;
        this.thumbnail = thumbnail;
        this.previews = previews;
        this.templateAssets = templateAssets;
        this.templatePreviews = templatePreviews;
        this.users = users;
        this.jobs = jobs;
        this.receivingMethods = receivingMethods;
        this.receivingMethod = receivingMethod;
        this.priceConstraints = priceConstraints.Value;
        this.cancelPurchaseFeedbacks = cancelPurchaseFeedbacks;
        this.authorization = authorization;
        this.captcha = captcha;
        this.activities = activities;
    }

    [HttpPost]
    [Authorize(AuthorizationPolicies.TemplateWrite)]
    public async Task<IActionResult> RegisterAsync([FromBody] RegisterTemplate request, CancellationToken cancellationToken = default)
    {
        // Validate request.
        if (!this.options.AllowedTemplateCultures.Contains(request.Culture))
        {
            return this.BadRequest();
        }

        foreach (var job in request.ApplicableJobs)
        {
            if (!await this.jobs.IsExistsAsync(job, cancellationToken))
            {
                return this.BadRequest();
            }
        }

        if (request.PreviewJob is { } previewJob)
        {
            if (!request.ApplicableJobs.Contains(previewJob))
            {
                return this.BadRequest();
            }
        }

        // Check if user allowed to register template.
        var user = await this.users.GetAsync(this.User, cancellationToken);

        if (user == null || !user.EmailVerified)
        {
            return this.Forbid();
        }

        if (await this.repository.CountRegistrationAsync(user.Id, cancellationToken) >= 50)
        {
            return this.Conflict();
        }

        // Register.
        var now = DateTime.Now;
        var registration = new Domain(
            Guid.NewGuid(),
            user.Id,
            request.Name,
            request.Description ?? string.Empty,
            request.Website,
            request.Culture,
            request.ApplicableJobs,
            request.Category,
            new Dictionary<CurrencyCode, decimal>(),
            0,
            null,
            now,
            now);

        await this.repository.RegisterAsync(registration, cancellationToken);

        // Create a workspace.
        var applicableData = Enumerable.Empty<string>();
        var renderOptions = new KeyedByTypeCollection<Cloudsume.Resume.TemplateRenderOptions>();
        var assets = new HashSet<Cloudsume.Resume.TemplateAsset>();

        await this.workspaces.CreateAsync(registration.Id, new(request.PreviewJob, applicableData, renderOptions, assets));

        return this.Ok(new TemplateRegistration(registration, Enumerable.Empty<Guid>(),  null, now));
    }

    [HttpGet]
    public async Task<IEnumerable<TemplateRegistration>> ListAsync([FromQuery] Guid? owner, CancellationToken cancellationToken = default)
    {
        var auth = await this.authorization.AuthorizeAsync(this.User, AuthorizationPolicies.TemplateRead);
        var userId = auth.Succeeded ? this.users.GetId(this.User) : Guid.Empty;

        // Load registrations.
        IEnumerable<Domain> registrations;

        if (owner != null)
        {
            registrations = await this.repository.ListRegistrationByOwnerAsync(owner.Value, cancellationToken);
        }
        else
        {
            registrations = await this.repository.ListRegistrationAsync(cancellationToken);
            registrations = registrations.Where(r => r.UnlistedReason is null);
        }

        // Filter and map to DTO.
        var result = new List<TemplateRegistration>();

        foreach (var r in registrations)
        {
            var owned = auth.Succeeded && r.UserId == userId;

            // Check category.
            switch (r.Category)
            {
                case RegistrationCategory.Free:
                case RegistrationCategory.Paid:
                    break;
                case RegistrationCategory.Private:
                    if (!owned)
                    {
                        continue;
                    }

                    break;
                default:
                    throw new NotImplementedException();
            }

            result.Add(await this.ToDtoAsync(r, owned, cancellationToken));
        }

        return result;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAsync([FromRoute] Guid id, CancellationToken cancellationToken = default)
    {
        // Load registration.
        var registration = await this.GetRegistrationAsync(id, cancellationToken);

        if (registration == null)
        {
            return this.NotFound();
        }

        // Check if the user is owner.
        bool owned;

        if (registration.Category == RegistrationCategory.Private)
        {
            owned = true;
        }
        else
        {
            var auth = await this.authorization.AuthorizeAsync(this.User, AuthorizationPolicies.TemplateRead);
            owned = auth.Succeeded && registration.UserId == this.users.GetId(this.User);
        }

        return this.Ok(await this.ToDtoAsync(registration, owned, cancellationToken));
    }

    [HttpPut("{id}/applicable-jobs")]
    [Authorize(AuthorizationPolicies.TemplateWrite)]
    public async Task<IActionResult> SetApplicableJobsAsync(
        [FromRoute] Guid id,
        [FromBody, Required, Unique, MinLength(1), MaxLength(10)] IReadOnlyCollection<Guid> jobs,
        CancellationToken cancellationToken = default)
    {
        // Check for any invalid jobs.
        foreach (var job in jobs)
        {
            if (!await this.jobs.IsExistsAsync(job, cancellationToken))
            {
                return this.BadRequest();
            }
        }

        // Load registration to see if template belong to user.
        var registration = await this.GetOwnRegistrationAsync(id, cancellationToken);

        if (registration == null)
        {
            return this.NotFound();
        }

        // Check if template already published.
        if (await this.repository.CountTemplateAsync(id, cancellationToken) != 0)
        {
            return this.Conflict();
        }

        // Update applicable jobs.
        await this.repository.SetApplicableJobsAsync(id, jobs, cancellationToken);
        await this.repository.SetUpdatedAsync(id, DateTime.Now);

        return this.NoContent();
    }

    [HttpPost("{id}/applicable-jobs")]
    [Authorize(AuthorizationPolicies.TemplateWrite)]
    public async Task<IActionResult> AddApplicableJobsAsync(
        [FromRoute] Guid id,
        [FromBody, Required, Unique, MinLength(1), MaxLength(10)] IReadOnlyCollection<Guid> jobs,
        CancellationToken cancellationToken = default)
    {
        // Load registration.
        var registration = await this.GetOwnRegistrationAsync(id, cancellationToken);

        if (registration == null)
        {
            return this.NotFound();
        }

        // Check if the specified jobs can be added.
        if (registration.ApplicableJobs.Count() + jobs.Count > 10 || jobs.Any(id => registration.ApplicableJobs.Contains(id)))
        {
            return this.Conflict();
        }

        foreach (var job in jobs)
        {
            if (!await this.jobs.IsExistsAsync(job, cancellationToken))
            {
                return this.BadRequest();
            }
        }

        // Update applicable jobs.
        await this.repository.SetApplicableJobsAsync(id, registration.ApplicableJobs.Concat(jobs), cancellationToken);
        await this.repository.SetUpdatedAsync(id, DateTime.Now);

        return this.NoContent();
    }

    [HttpPost("{id}/cancel-purchase-feedbacks")]
    [Authorize(AuthorizationPolicies.TemplateLicenseWrite)]
    public async Task<IActionResult> CreateCancelPurchaseFeedbackAsync(
        [FromRoute] Guid id,
        [FromBody, RequireDefined] CancelPurchaseReason reason,
        CancellationToken cancellationToken = default)
    {
        // Load registration.
        var registration = await this.GetRegistrationAsync(id, cancellationToken);

        if (registration is null)
        {
            return this.NotFound();
        }

        // Load pending license.
        var userId = this.users.GetId(this.User);
        var license = await this.licenses.GetAsync(userId, id, cancellationToken);

        if (license is null || license.Payment is null || license.Status != TemplateLicenseStatus.WaitingPayment)
        {
            return this.Conflict();
        }

        // Cancel the purchase so the user cannot spam this endpoint.
        await this.receivingMethod.CancelPaymentAsync(license.Payment, PaymentCancelReason.PayerRequested, cancellationToken);
        await this.CancelPurchaseAsync(license);

        // Store the feedback.
        await this.cancelPurchaseFeedbacks.CreateAsync(new(
            id,
            Ulid.Generate(),
            userId,
            license.Payment,
            reason,
            this.GetRemoteIp(),
            this.GetUserAgent()));

        return this.NoContent();
    }

    [HttpPut("{id}/category")]
    [Authorize(AuthorizationPolicies.TemplateWrite)]
    public async Task<IActionResult> SetRegistrationCategoryAsync(
        [FromRoute] Guid id,
        [FromBody, RequireDefined] RegistrationCategory category,
        CancellationToken cancellationToken = default)
    {
        // Load registration.
        var registration = await this.GetOwnRegistrationAsync(id, cancellationToken);

        if (registration == null)
        {
            return this.NotFound();
        }

        // Check if category can be changed to the specified value.
        if (registration.Category == category)
        {
            return this.BadRequest();
        }

        switch (registration.Category)
        {
            case RegistrationCategory.Free:
            case RegistrationCategory.Paid:
                if (category == RegistrationCategory.Private)
                {
                    return this.Conflict();
                }

                break;
            case RegistrationCategory.Private:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(category));
        }

        if (category == RegistrationCategory.Paid && !await this.CanPublishPaidTemplateAsync(registration, cancellationToken))
        {
            return this.Conflict();
        }

        // Update category.
        await this.repository.SetRegistrationCategoryAsync(id, category, cancellationToken);
        await this.repository.SetUpdatedAsync(id, DateTime.Now);

        return this.NoContent();
    }

    [HttpPut("{id}/description")]
    [Authorize(AuthorizationPolicies.TemplateWrite)]
    public async Task<IActionResult> SetDescriptionAsync(
        [FromRoute] Guid id,
        [FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Allow), StringLength(10000, MinimumLength = 1)] string? description,
        CancellationToken cancellationToken = default)
    {
        // Load registration to see if template belong to user.
        var registration = await this.GetOwnRegistrationAsync(id, cancellationToken);

        if (registration == null)
        {
            return this.NotFound();
        }

        // Update description.
        await this.repository.SetDescriptionAsync(id, description ?? string.Empty, cancellationToken);
        await this.repository.SetUpdatedAsync(id, DateTime.Now);

        return this.NoContent();
    }

    [HttpGet("{id}/license")]
    [Authorize(AuthorizationPolicies.TemplateLicenseRead)]
    public async Task<IActionResult> GetLicenseAsync([FromRoute] Guid id, CancellationToken cancellationToken = default)
    {
        var userId = this.users.GetId(this.User);
        var license = await this.licenses.GetAsync(userId, id, cancellationToken);

        if (license == null)
        {
            return this.NoContent();
        }
        else if (license.Status == TemplateLicenseStatus.WaitingPayment)
        {
            var payment = license.Payment;

            if (payment == null)
            {
                throw new DataCorruptionException(license, "Status is WaitingPayment but Payment is null.");
            }

            var status = await this.receivingMethod.GetStatusAsync(payment, cancellationToken);

            switch (status)
            {
                case PaymentMethodStatus.Created:
                    return this.NoContent();
                case PaymentMethodStatus.Succeeded:
                    license.Status = TemplateLicenseStatus.Valid;
                    license.UpdatedAt = DateTime.Now;

                    await this.licenses.SetStatusAsync(license.RegistrationId, license.Id, license.Status, cancellationToken);
                    await this.licenses.SetUpdatedAsync(license.RegistrationId, license.Id, license.UpdatedAt);
                    await this.activities.WriteAsync(new CompletePurchaseTemplateActivity(userId, id, this.GetRemoteIp(), this.GetUserAgent()));
                    break;
                case PaymentMethodStatus.Canceled:
                    await this.CancelPurchaseAsync(license, cancellationToken);
                    return this.NoContent();
                default:
                    throw new NotImplementedException($"Status {status} is not implemented.");
            }
        }

        return this.Ok(new TemplateLicense(license));
    }

    [HttpPut("{id}/name")]
    [Authorize(AuthorizationPolicies.TemplateWrite)]
    public async Task<IActionResult> SetNameAsync(
        [FromRoute] Guid id,
        [FromBody, Required, MaxLength(100)] string name,
        CancellationToken cancellationToken = default)
    {
        // Load registration to see if template belong to user.
        var registration = await this.GetOwnRegistrationAsync(id, cancellationToken);

        if (registration == null)
        {
            return this.NotFound();
        }

        // Update name.
        await this.repository.SetNameAsync(id, name, cancellationToken);
        await this.repository.SetUpdatedAsync(id, DateTime.Now);

        return this.NoContent();
    }

    [HttpGet("{id}/payment-method")]
    [Authorize(AuthorizationPolicies.TemplateLicenseWrite)]
    public async Task<IActionResult> GetPaymentMethodAsync(
        [FromRoute] Guid id,
        [FromQuery, BindRequired] CurrencyCode currency,
        [FromQuery, BindRequired, MinLength(1)] string captcha,
        CancellationToken cancellationToken = default)
    {
        // Check if user already bough a license.
        var userId = this.users.GetId(this.User);
        var license = await this.licenses.GetAsync(userId, id, cancellationToken);

        if (license != null && license.Status == TemplateLicenseStatus.Valid)
        {
            return this.NotFound();
        }

        // Check if required information is present.
        var registration = await this.GetRegistrationAsync(id, cancellationToken);
        decimal price;

        if (registration == null || registration.Prices.Count == 0)
        {
            return this.NotFound();
        }
        else if (!registration.Prices.TryGetValue(currency, out price))
        {
            return this.NoContent();
        }

        // Validate CAPTCHA.
        var captchaResult = await this.captcha.ValidateReCaptchaAsync(
            captcha,
            "template_payment_method",
            this.HttpContext.Connection.RemoteIpAddress,
            null,
            cancellationToken);

        if (!captchaResult)
        {
            return this.BadRequest();
        }

        // Select a method.
        var payment = new Cloudsume.Financial.PaymentInfo(this.User, currency, price, registration.Name);
        var fee = registration.UserId == Guid.Empty ? 0m : CurrencyInfo.Get(currency).Round(price * 0.2m);
        Cloudsume.Financial.PaymentMethod? method = null;

        foreach (var available in await this.receivingMethods.ListAsync(registration.UserId, cancellationToken))
        {
            method = await this.receivingMethod.CreatePaymentMethodAsync(available, payment, fee);

            if (method != null)
            {
                break;
            }
        }

        if (method == null)
        {
            return this.NotFound();
        }

        // Set a license status to waiting for payment.
        if (license != null)
        {
            await this.licenses.SetPaymentAsync(license.RegistrationId, license.Id, method);
            await this.licenses.SetStatusAsync(license.RegistrationId, license.Id, TemplateLicenseStatus.WaitingPayment);
            await this.licenses.SetUpdatedAsync(license.RegistrationId, license.Id, DateTime.Now);
        }
        else
        {
            await this.licenses.CreateAsync(new(Ulid.Generate(), id, userId, method, TemplateLicenseStatus.WaitingPayment, DateTime.Now));
        }

        // Write the activity.
        await this.activities.WriteAsync(new StartPurchaseTemplateActivity(userId, id, currency, price, this.GetRemoteIp(), this.GetUserAgent()));

        return this.Ok(PaymentMethod.From(method, price));
    }

    [HttpPut("{id}/prices")]
    [Authorize(AuthorizationPolicies.TemplateWrite)]
    public async Task<IActionResult> SetPricesAsync(
        [FromRoute] Guid id,
        [FromBody, MinLength(1)] IReadOnlyDictionary<CurrencyCode, decimal> prices,
        CancellationToken cancellationToken = default)
    {
        // Check price constraints.
        foreach (var (currency, price) in prices)
        {
            if (!CurrencyInfo.Get(currency).IsValidAmount(price))
            {
                return this.BadRequest();
            }
            else if (!this.priceConstraints.TryGetValue(currency.Value, out var constraint))
            {
                return this.BadRequest();
            }
            else if (price < constraint.Min || price > constraint.Max)
            {
                return this.BadRequest();
            }
        }

        // Check if registration belong to current user.
        if (await this.GetOwnRegistrationAsync(id, cancellationToken) == null)
        {
            return this.NotFound();
        }

        // Update prices. We don't need to check registration category due to we want user to keep prices data between category changed.
        await this.repository.SetPricesAsync(id, prices, cancellationToken);
        await this.repository.SetUpdatedAsync(id, DateTime.Now);

        return this.NoContent();
    }

    [HttpPost("{id}/releases")]
    [Authorize(AuthorizationPolicies.TemplateWrite)]
    public async Task<IActionResult> CreateReleaseAsync(
        [FromRoute] Guid id,
        [FromBody] ReleaseTemplate release,
        CancellationToken cancellationToken = default)
    {
        // Load registration.
        var registration = await this.GetOwnRegistrationAsync(id, cancellationToken);

        if (registration == null)
        {
            return this.NotFound();
        }

        // Check if new release is allowed.
        if (registration.Category == RegistrationCategory.Paid && !await this.CanPublishPaidTemplateAsync(registration, cancellationToken))
        {
            return this.NotFound();
        }

        if (await this.repository.CountTemplateAsync(id, cancellationToken) >= 50)
        {
            return this.Conflict();
        }

        // Transfer workspace data.
        var workspace = await this.workspaces.GetAsync(id, cancellationToken);

        if (workspace == null || !workspace.HasRequiredAssets)
        {
            return this.NotFound();
        }

        var template = new Cloudsume.Resume.Template(
            Ulid.Generate(),
            id,
            workspace.ApplicableData,
            workspace.RenderOptions,
            registration.Category,
            release.Note,
            0);

        // Build to see if it is buildable then generate previews.
        await using var compile = await this.GeneratePreviewAsync(registration, workspace, cancellationToken);

        await this.templatePreviews.CreateAsync(template.Id, this.thumbnail.GenerateAsync(compile.PDF, cancellationToken), cancellationToken);

        // Copy assets from workspace.
        await foreach (var asset in this.ReadWorkspaceAssetsAsync(id, workspace))
        {
            await this.templateAssets.WriteAsync(template.Id, asset);
        }

        // All template data has been created, now we can write the database.
        await this.repository.CreateTemplateAsync(template);
        await this.repository.SetUpdatedAsync(template.RegistrationId, DateTime.Now);

        return this.Ok(template.Id);
    }

    [HttpGet("{id}/releases")]
    public async Task<IActionResult> ListReleaseAsync([FromRoute] Guid id, CancellationToken cancellationToken = default)
    {
        var templates = await this.ListAuthorizedTemplateAsync(id, cancellationToken);
        var response = templates.Select(ToDto).ToArray();

        return this.Ok(response);

        TemplateSummary ToDto(Cloudsume.Resume.Template domain)
        {
            var preview = this.Url.ActionLink("GetPreview", "Template", values: new { id = domain.Id, page = 0 });

            if (preview == null)
            {
                throw new Exception($"Failed to get a preview link for template {domain.Id}.");
            }

            return new(domain, preview);
        }
    }

    [HttpGet("{id}/templates")]
    public async Task<IActionResult> ListTemplateAsync([FromRoute] Guid id, CancellationToken cancellationToken = default)
    {
        var templates = await this.ListAuthorizedTemplateAsync(id, cancellationToken);
        var response = templates.Select(t => t.Id).ToArray();

        return this.Ok(response);
    }

    [HttpPut("{id}/website")]
    [Authorize(AuthorizationPolicies.TemplateWrite)]
    public async Task<IActionResult> SetWebsiteAsync(
        [FromRoute] Guid id,
        [FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Allow), Uri(Schemes = UriSchemes.HTTP | UriSchemes.HTTPS, Kind = UriKind.Absolute)] Uri? website,
        CancellationToken cancellationToken = default)
    {
        // Load registration to see if the template belong to current user.
        var registration = await this.GetOwnRegistrationAsync(id, cancellationToken);

        if (registration == null)
        {
            return this.NotFound();
        }

        // Update website.
        await this.repository.SetWebsiteAsync(id, website, cancellationToken);
        await this.repository.SetUpdatedAsync(id, DateTime.Now);

        return this.NoContent();
    }

    [HttpGet("{id}/workspace")]
    [Authorize(AuthorizationPolicies.TemplateRead)]
    public async Task<IActionResult> GetWorkspaceAsync([FromRoute] Guid id, CancellationToken cancellationToken = default)
    {
        // Check if registration belong to current user.
        if (await this.GetOwnRegistrationAsync(id, cancellationToken) == null)
        {
            return this.NotFound();
        }

        // Load workspace.
        var workspace = await this.workspaces.GetAsync(id, cancellationToken);

        workspace ??= new Cloudsume.Resume.TemplateWorkspace(
            null,
            Enumerable.Empty<string>(),
            new KeyedByTypeCollection<Cloudsume.Resume.TemplateRenderOptions>(),
            new HashSet<Cloudsume.Resume.TemplateAsset>());

        return this.Ok(new TemplateWorkspace(workspace));
    }

    [HttpPut("{id}/workspace/applicable-data")]
    [Authorize(AuthorizationPolicies.TemplateWrite)]
    public async Task<IActionResult> WriteWorkspaceApplicableDataAsync(
        [FromRoute] Guid id,
        [FromBody, Unique] IEnumerable<string> data,
        CancellationToken cancellationToken = default)
    {
        foreach (var type in data)
        {
            if (!this.managers.ContainsKey(type))
            {
                return this.BadRequest();
            }
        }

        // Check if registration belong to current user.
        if (await this.GetOwnRegistrationAsync(id, cancellationToken) == null)
        {
            return this.NotFound();
        }

        await this.workspaces.UpdateApplicableDataAsync(id, data, cancellationToken);
        await this.repository.SetUpdatedAsync(id, DateTime.Now);

        return this.NoContent();
    }

    [HttpPut("{id}/workspace/assets/{**name}")]
    [Authorize(AuthorizationPolicies.TemplateWrite)]
    public async Task<IActionResult> WriteWorkspaceAssetAsync(
        [FromRoute] Guid id,
        [FromRoute] AssetName name,
        CancellationToken cancellationToken = default)
    {
        // Sanity check.
        if (this.Request.ContentLength is not { } size)
        {
            return this.StatusCode(411); // Length Required.
        }

        if (this.Request.Headers["Content-Encoding"] != StringValues.Empty)
        {
            // When Content-Encoding is specified Content-Length will not be the actual size of body; we don't want that.
            return this.BadRequest();
        }

        if (name == "main.stg" && size > 100000)
        {
            return this.BadRequest();
        }

        // Check if registration belong to the user.
        if (await this.GetOwnRegistrationAsync(id, cancellationToken) == null)
        {
            return this.NotFound();
        }

        // Check if writing result going to exceed quota.
        var workspace = await this.workspaces.GetAsync(id, cancellationToken);
        var files = 0;
        var usage = 0L;
        int? current = null;

        if (workspace != null)
        {
            foreach (var asset in workspace.Assets)
            {
                files += 1;
                usage += asset.Size;

                if (asset.Name == name)
                {
                    current = asset.Size;
                }
            }
        }

        if (current != null)
        {
            var add = size - current.Value;

            if (add > 0 && usage + add > MaxWorkspace)
            {
                return this.Conflict();
            }
        }
        else if (files >= 100 || usage + size > MaxWorkspace)
        {
            return this.Conflict();
        }

        // Write asset.
        await this.workspaceAssets.WriteAsync(id, name, this.Request.Body, size, cancellationToken);

        // Update workspace.
        var now = DateTime.Now;
        var update = new Cloudsume.Resume.TemplateAsset(name, Convert.ToInt32(size), now);

        if (workspace == null)
        {
            var applicableData = Enumerable.Empty<string>();
            var renderOptions = new KeyedByTypeCollection<Cloudsume.Resume.TemplateRenderOptions>();
            var assets = new HashSet<Cloudsume.Resume.TemplateAsset>() { update };

            await this.workspaces.CreateAsync(id, new(null, applicableData, renderOptions, assets));
        }
        else
        {
            await this.workspaces.UpdateAssetAsync(id, update);
        }

        await this.repository.SetUpdatedAsync(id, now);

        // Create response.
        var response = new TemplateAsset(update);

        if (current == null)
        {
            var url = this.Url.ActionLink("GetWorkspaceAsset", values: new { id, name = name.Value });

            if (url == null)
            {
                throw new Exception($"Failed to get a URL to {nameof(this.GetWorkspaceAssetAsync)}.");
            }

            return this.Created(url, response);
        }
        else
        {
            return this.Ok(response);
        }
    }

    [HttpGet("{id}/workspace/assets/{**name}")]
    [Authorize(AuthorizationPolicies.TemplateRead)]
    public async Task<IActionResult> GetWorkspaceAssetAsync(
        [FromRoute] Guid id,
        [FromRoute] AssetName name,
        CancellationToken cancellationToken = default)
    {
        // Check if registration belong to current user.
        if (await this.GetOwnRegistrationAsync(id, cancellationToken) == null)
        {
            return this.NotFound();
        }

        // Load asset.
        var asset = await this.workspaceAssets.GetAsync(id, name, cancellationToken);

        if (asset == null)
        {
            return this.NotFound();
        }

        // Create response.
        try
        {
            return this.File(asset, "application/octet-stream", false);
        }
        catch
        {
            await asset.DisposeAsync();
            throw;
        }
    }

    [HttpDelete("{id}/workspace/assets/{**name}")]
    [Authorize(AuthorizationPolicies.TemplateWrite)]
    public async Task<IActionResult> DeleteWorkspaceAssetAsync(
        [FromRoute] Guid id,
        [FromRoute] AssetName name,
        CancellationToken cancellationToken = default)
    {
        // Check if registration belong to current user.
        if (await this.GetOwnRegistrationAsync(id, cancellationToken) == null)
        {
            return this.NotFound();
        }

        // Execute deletion.
        await this.workspaces.DeleteAssetAsync(id, name, cancellationToken);
        await this.workspaceAssets.DeleteAsync(id, name);
        await this.repository.SetUpdatedAsync(id, DateTime.Now);

        return this.NoContent();
    }

    [HttpPut("{id}/workspace/preview-job")]
    [Authorize(AuthorizationPolicies.TemplateWrite)]
    public async Task<IActionResult> WriteWorkspacePreviewJobAsync(
        [FromRoute] Guid id,
        [FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Allow)] Guid? jobId,
        CancellationToken cancellationToken = default)
    {
        // Load the registration.
        var registration = await this.GetOwnRegistrationAsync(id, cancellationToken);

        if (registration == null)
        {
            return this.NotFound();
        }

        // Check if the specified job valid. Just checking with ApplicableJobs is enough due to we already make sure ApplicableJobs contain valid identifiers.
        if (jobId != null && !registration.ApplicableJobs.Contains(jobId.Value))
        {
            return this.BadRequest();
        }

        // Execute update.
        await this.workspaces.UpdatePreviewJobAsync(id, jobId, cancellationToken);
        await this.repository.SetUpdatedAsync(id, DateTime.Now);

        return this.NoContent();
    }

    [HttpGet("{id}/workspace/previews")]
    public async Task<IActionResult> GetWorkspacePreviewsAsync([FromRoute] Guid id, [FromQuery] bool rebuild, CancellationToken cancellationToken = default)
    {
        // Load registration.
        var registration = await this.GetOwnRegistrationAsync(id, cancellationToken);
        string? source;
        int pages;

        if (registration == null)
        {
            return this.NotFound();
        }

        if (rebuild)
        {
            // Build a new preview and return it.
            var auth = await this.authorization.AuthorizeAsync(this.User, AuthorizationPolicies.TemplateWrite);

            if (!auth.Succeeded)
            {
                return this.NotFound();
            }

            // Load workspace.
            var workspace = await this.workspaces.GetAsync(id, cancellationToken);

            if (workspace == null || !workspace.HasRequiredAssets)
            {
                return this.NotFound();
            }

            // Generate previews.
            CompileResult compile;

            try
            {
                compile = await this.GeneratePreviewAsync(registration, workspace, cancellationToken);
            }
            catch (TemplateSyntaxException ex)
            {
                return BuildError(null, ex.Message, ex.Line);
            }
            catch (TemplateException ex)
            {
                return BuildError(null, ex.Message, null);
            }
            catch (CompileException ex)
            {
                return BuildError(ex.Input, ex.Message, null);
            }

            // Create thumbnails.
            await using (compile)
            {
                var previews = this.thumbnail.GenerateAsync(compile.PDF, cancellationToken);

                source = compile.Source;
                pages = await this.previews.UpdateAsync(id, previews, cancellationToken);
            }

            await this.repository.SetUpdatedAsync(id, DateTime.Now);

            IActionResult BuildError(string? source, string log, int? line) => this.StatusCode(500, new WorkspaceBuildError(source, log, line));
        }
        else
        {
            // Return previous build previews.
            var auth = await this.authorization.AuthorizeAsync(this.User, AuthorizationPolicies.TemplateRead);

            if (!auth.Succeeded)
            {
                return this.NotFound();
            }

            source = null;
            pages = await this.previews.GetPageCountAsync(id, cancellationToken);
        }

        // Create response.
        var thumbnails = new List<string>();

        for (var page = 0; page < pages; page++)
        {
            var url = this.Url.ActionLink("GetWorkspacePreview", values: new { id, page });

            if (url == null)
            {
                throw new Exception($"Failed to get a URL to {nameof(this.GetWorkspacePreviewAsync)}.");
            }

            thumbnails.Add(url);
        }

        return this.Ok(new WorkspacePreview(source, thumbnails));
    }

    [HttpGet("{id}/workspace/previews/{page}")]
    [Authorize(AuthorizationPolicies.TemplateRead)]
    public async Task<IActionResult> GetWorkspacePreviewAsync(
        [FromRoute] Guid id,
        [FromRoute, NonNegative] int page,
        CancellationToken cancellationToken = default)
    {
        // Check if registration belong to current user.
        if (await this.GetOwnRegistrationAsync(id, cancellationToken) == null)
        {
            return this.NotFound();
        }

        // Load preview.
        var preview = await this.previews.GetAsync(id, page, cancellationToken);

        if (preview == null)
        {
            return this.NotFound();
        }

        // Create response.
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

    [HttpPut("{id}/workspace/render-options/education")]
    [Authorize(AuthorizationPolicies.TemplateWrite)]
    public async Task<IActionResult> WriteWorkspaceEducationOptionsAsync(
        [FromRoute] Guid id,
        [FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Allow)] EducationOptions? options,
        CancellationToken cancellationToken = default)
    {
        // Check if registration belong to current user.
        if (await this.GetOwnRegistrationAsync(id, cancellationToken) == null)
        {
            return this.NotFound();
        }

        // Update.
        Candidate.Server.Resume.Data.EducationRenderOptions? domain;

        if (options != null)
        {
            domain = new(options.DescriptionParagraph, options.DescriptionListOptions);
        }
        else
        {
            domain = null;
        }

        await this.workspaces.UpdateRenderOptionsAsync(id, domain, cancellationToken);
        await this.repository.SetUpdatedAsync(id, DateTime.Now);

        return this.NoContent();
    }

    [HttpPut("{id}/workspace/render-options/experience")]
    [Authorize(AuthorizationPolicies.TemplateWrite)]
    public async Task<IActionResult> WriteWorkspaceExperienceOptionsAsync(
        [FromRoute] Guid id,
        [FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Allow)] ExperienceOptions? options,
        CancellationToken cancellationToken = default)
    {
        // Check if registration belong to current user.
        if (await this.GetOwnRegistrationAsync(id, cancellationToken) == null)
        {
            return this.NotFound();
        }

        // Update.
        Candidate.Server.Resume.Data.ExperienceRenderOptions? domain;

        if (options != null)
        {
            domain = new(options.DescriptionParagraph, options.DescriptionListOptions);
        }
        else
        {
            domain = null;
        }

        await this.workspaces.UpdateRenderOptionsAsync(id, domain, cancellationToken);
        await this.repository.SetUpdatedAsync(id, DateTime.Now);

        return this.NoContent();
    }

    [HttpPut("{id}/workspace/render-options/skill")]
    [Authorize(AuthorizationPolicies.TemplateWrite)]
    public async Task<IActionResult> WriteWorkspaceSkillOptionsAsync(
        [FromRoute] Guid id,
        [FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Allow)] SkillOptions? options,
        CancellationToken cancellationToken = default)
    {
        // Check if registration belong to current user.
        if (await this.GetOwnRegistrationAsync(id, cancellationToken) == null)
        {
            return this.NotFound();
        }

        // Update.
        Candidate.Server.Resume.Data.SkillRenderOptions? domain;

        if (options != null)
        {
            domain = new(options.Grouping);
        }
        else
        {
            domain = null;
        }

        await this.workspaces.UpdateRenderOptionsAsync(id, domain, cancellationToken);
        await this.repository.SetUpdatedAsync(id, DateTime.Now);

        return this.NoContent();
    }

    private async Task<TemplateRegistration> ToDtoAsync(Domain domain, bool owner, CancellationToken cancellationToken = default)
    {
        var packs = await this.packs.GetPacksAsync(domain.Id, cancellationToken);
        var latest = await this.GetLatestTemplateAsync(domain.Id, owner, cancellationToken);

        return new(domain, packs.Select(p => p.Id), latest?.Id, owner ? domain.UpdatedAt : domain.CreatedAt);
    }

    private async Task<Domain?> GetRegistrationAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var registration = await this.repository.GetRegistrationAsync(id, cancellationToken);

        if (registration == null)
        {
            return null;
        }

        if (registration.Category == RegistrationCategory.Private)
        {
            var auth = await this.authorization.AuthorizeAsync(this.User, AuthorizationPolicies.TemplateRead);

            if (!auth.Succeeded || registration.UserId != this.users.GetId(this.User))
            {
                return null;
            }
        }

        return registration;
    }

    private async Task<Domain?> GetOwnRegistrationAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var registration = await this.repository.GetRegistrationAsync(id, cancellationToken);

        if (registration == null || !this.User.Identities.First().IsAuthenticated || registration.UserId != this.users.GetId(this.User))
        {
            return null;
        }

        return registration;
    }

    private async Task<Cloudsume.Resume.Template?> GetLatestTemplateAsync(
        Guid registrationId,
        bool includePrivate,
        CancellationToken cancellationToken = default)
    {
        foreach (var t in await this.repository.ListTemplateAsync(registrationId, cancellationToken))
        {
            switch (t.Category)
            {
                case RegistrationCategory.Free:
                case RegistrationCategory.Paid:
                    break;
                case RegistrationCategory.Private:
                    if (!includePrivate)
                    {
                        continue;
                    }

                    break;
                default:
                    throw new NotImplementedException();
            }

            return t;
        }

        return null;
    }

    private async Task<IEnumerable<Cloudsume.Resume.Template>> ListAuthorizedTemplateAsync(Guid registrationId, CancellationToken cancellationToken = default)
    {
        var registration = await this.repository.GetRegistrationAsync(registrationId, cancellationToken);

        if (registration == null)
        {
            return Enumerable.Empty<Cloudsume.Resume.Template>();
        }

        var auth = await this.authorization.AuthorizeAsync(this.User, AuthorizationPolicies.TemplateRead);
        var userId = new Lazy<Guid>(() => this.users.GetId(this.User));
        var templates = await this.repository.ListTemplateAsync(registrationId, cancellationToken);

        return templates.Where(IsVisible).ToArray();

        bool IsVisible(Cloudsume.Resume.Template template) => template.Category switch
        {
            RegistrationCategory.Free => true,
            RegistrationCategory.Private => auth.Succeeded && registration.UserId == userId.Value,
            RegistrationCategory.Paid => true,
            _ => throw new NotImplementedException(),
        };
    }

    private async Task CancelPurchaseAsync(Cloudsume.Resume.TemplateLicense license, CancellationToken cancellationToken = default)
    {
        if (license.Status != TemplateLicenseStatus.WaitingPayment)
        {
            throw new ArgumentException($"The status of license is not {TemplateLicenseStatus.WaitingPayment}.", nameof(license));
        }

        var userId = this.users.GetId(this.User);

        await this.licenses.DeleteAsync(license.RegistrationId, license.Id, cancellationToken);
        await this.activities.WriteAsync(new CancelPurchaseTemplateActivity(userId, license.RegistrationId, this.GetRemoteIp(), this.GetUserAgent()));
    }

    private async IAsyncEnumerable<AssetFile> ReadWorkspaceAssetsAsync(
        Guid registrationId,
        Cloudsume.Resume.TemplateWorkspace workspace,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        foreach (var asset in workspace.Assets)
        {
            var content = await this.workspaceAssets.GetAsync(registrationId, asset.Name, cancellationToken);

            if (content == null)
            {
                throw new DataCorruptionException(workspace, $"Asset {asset.Name} has no content.");
            }

            AssetFile file;

            try
            {
                file = new(asset.Name, asset.Size, content);
            }
            catch
            {
                await content.DisposeAsync();
                throw;
            }

            yield return file;
        }
    }

    private async Task<bool> CanPublishPaidTemplateAsync(Cloudsume.Resume.TemplateRegistration registration, CancellationToken cancellationToken = default)
    {
        // Prices need to be defined for paid template.
        if (registration.Prices.Count == 0)
        {
            return false;
        }

        // Payment receiving method is required for paid template.
        var payments = await this.receivingMethods.ListAsync(registration.UserId, cancellationToken);

        if (!payments.Any())
        {
            return false;
        }

        // Required a working receiving method.
        foreach (var method in payments)
        {
            var status = await this.receivingMethod.GetStatusAsync(method, cancellationToken);

            if (status >= ReceivingMethodStatus.Ready)
            {
                return true;
            }
        }

        return false;
    }

    private async Task<CompileResult> GeneratePreviewAsync(
        Domain registration,
        Cloudsume.Resume.TemplateWorkspace workspace,
        CancellationToken cancellationToken = default)
    {
        var userId = registration.UserId;
        var targetJob = workspace.PreviewJob;
        var culture = registration.Culture;

        // Load sample data for target job and culture.
        var samples = new List<Candidate.Server.Resume.ResumeData>();

        foreach (var type in workspace.ApplicableData)
        {
            // Check if user has specified preview job.
            if (targetJob is { } jobId)
            {
                // Try data for target job first.
                if (await LoadSampleDataAsync(userId, jobId, culture, type))
                {
                    continue;
                }

                // Try default job if no data for target job.
                if (await LoadSampleDataAsync(userId, Guid.Empty, culture, type))
                {
                    continue;
                }
            }

            // Fallback to system data if no user-defined data or user did not specify preview job.
            if (await LoadSampleDataAsync(Guid.Empty, Guid.Empty, CultureInfo.InvariantCulture, type))
            {
                continue;
            }

            throw new ArgumentException($"No sample data for '{type}' available for template {registration.Id}.", nameof(registration));
        }

        // Build.
        return await this.compiler.CompileAsync(
            culture,
            this.ReadWorkspaceAssetsAsync(registration.Id, workspace, cancellationToken),
            workspace.RenderOptions,
            samples,
            cancellationToken);

        async Task<bool> LoadSampleDataAsync(Guid userId, Guid jobId, CultureInfo culture, string type)
        {
            var result = await this.sampleData.LoadAsync(userId, jobId, culture, type, cancellationToken);

            if (result is null)
            {
                return false;
            }

            samples.AddRange(this.aggregator.Aggregate(result.Item1, result.Item2));

            return true;
        }
    }
}
