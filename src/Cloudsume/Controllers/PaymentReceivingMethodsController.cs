namespace Cloudsume.Controllers;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Cloudsume.Models;
using Cloudsume.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using NetCaptcha;
using IReceivingMethodRepository = Cloudsume.Financial.IReceivingMethodRepository;
using IReceivingMethodService = Cloudsume.Financial.IReceivingMethodService;
using IStripeClient = global::Stripe.IStripeClient;
using IUserRepository = Cloudsume.Identity.IUserRepository;
using User = Cloudsume.Identity.User;

[ApiController]
[Route("payment-receiving-methods")]
public sealed class PaymentReceivingMethodsController : ControllerBase
{
    private readonly PaymentReceivingOptions options;
    private readonly IReceivingMethodRepository repository;
    private readonly IReceivingMethodService service;
    private readonly IUserRepository users;
    private readonly IStripeClient stripe;
    private readonly ICaptchaService captcha;
    private readonly IWebHostEnvironment host;

    public PaymentReceivingMethodsController(
        IOptions<PaymentReceivingOptions> options,
        IReceivingMethodRepository repository,
        IReceivingMethodService service,
        IUserRepository users,
        IStripeClient stripe,
        ICaptchaService captcha,
        IWebHostEnvironment host)
    {
        this.options = options.Value;
        this.repository = repository;
        this.service = service;
        this.users = users;
        this.stripe = stripe;
        this.captcha = captcha;
        this.host = host;
    }

    [HttpPost]
    [Authorize(AuthorizationPolicies.PaymentReceivingMethodWrite)]
    public async Task<IActionResult> CreateAsync([FromBody] CreatePaymentReceivingMethod request, CancellationToken cancellationToken = default)
    {
        // Check if user can create a specified method.
        var user = await this.users.GetAsync(this.User, cancellationToken);

        if (user == null || !user.EmailVerified)
        {
            return this.Forbid();
        }

        var currents = await this.repository.ListAsync(user.Id, cancellationToken);
        var type = request.GetType().GetCustomAttribute<CorrespondingDomainAttribute>()?.Type;

        if (type == null)
        {
            throw new Exception($"No {typeof(CorrespondingDomainAttribute)} is applied to {request.GetType()}.");
        }

        if (currents.Any(p => p.GetType() == type))
        {
            return this.Conflict();
        }

        // Create method.
        var method = request switch
        {
            CreateStripeReceivingMethod r => await this.CreateStripeAsync(user, r, cancellationToken),
            _ => throw new NotImplementedException($"No implementation for {request.GetType()}."),
        };

        if (method == null)
        {
            return this.BadRequest();
        }

        return this.Ok(await this.ToDtoAsync(method));
    }

    [HttpGet]
    [Authorize(AuthorizationPolicies.PaymentReceivingMethodRead)]
    public async Task<IActionResult> ListAsync(CancellationToken cancellationToken = default)
    {
        var userId = this.users.GetId(this.User);
        var methods = await this.repository.ListAsync(userId, cancellationToken);
        var response = new List<object>();

        foreach (var method in methods)
        {
            response.Add(await this.ToDtoAsync(method, cancellationToken));
        }

        return this.Ok(response);
    }

    [HttpGet("{id}/setup-uri")]
    [Authorize(AuthorizationPolicies.PaymentReceivingMethodWrite)]
    public async Task<IActionResult> GetSetupUriAsync(
        [FromRoute] Guid id,
        [FromQuery(Name = "return_uri"), BindRequired] Uri returnUri,
        [FromQuery, BindRequired, MinLength(1)] string captcha,
        CancellationToken cancellationToken = default)
    {
        if (!this.options.AllowedSetupReturnUris.Contains(returnUri))
        {
            this.ModelState.TryAddModelError("return_uri", "The value is not allowed URI.");
            return this.BadRequest(this.ModelState);
        }

        // Validate CAPTCHA.
        var captchaResult = await this.captcha.ValidateReCaptchaAsync(
            captcha,
            "payment_receiving_method_setup_uri",
            this.HttpContext.Connection.RemoteIpAddress,
            null,
            cancellationToken);

        if (!captchaResult)
        {
            this.ModelState.TryAddModelError("captcha", "Invalid CAPTCHA challenge.");
            return this.BadRequest(this.ModelState);
        }

        // Load target method.
        var userId = this.users.GetId(this.User);
        var method = await this.repository.GetAsync(userId, id, cancellationToken);

        if (method == null)
        {
            return this.NotFound();
        }

        // Get URI.
        var uri = await this.service.GetSetupUriAsync(method, returnUri, cancellationToken);

        if (uri == null)
        {
            return this.NotFound();
        }

        return this.Ok(uri);
    }

    private async Task<Cloudsume.Stripe.ReceivingMethod?> CreateStripeAsync(
        User owner,
        CreateStripeReceivingMethod request,
        CancellationToken cancellationToken = default)
    {
        // Setup create account request.
        var create = new global::Stripe.AccountCreateOptions()
        {
            Type = "express",
            Country = request.Country.Name,
            Email = owner.Email.Address,
            Capabilities = new()
            {
                Transfers = new()
                {
                    Requested = true,
                },
            },
            Metadata = new()
            {
                { "owner", owner.Id.ToString() },
            },
        };

        // The following countries have some restrictions and we cannot allow them to provide the services on our platform:
        //
        // - IN: To be able to receive a money outside India the entity must have a license to export goods from India.
        // - JP: https://stripe.com/docs/connect/cross-border-payouts/special-requirements#japan
        switch (create.Country)
        {
            case "EE": // Estonia
            case "NG": // Nigeria
            case "SG": // Singapore
            case "TH": // Thailand
                create.TosAcceptance = new() { ServiceAgreement = "recipient" };
                break;
            case "US": // United States
                // The recipient ToS agreement is not supported for platforms in US creating accounts in US.
                create.TosAcceptance = new() { ServiceAgreement = "full" };
                create.Capabilities.TaxReportingUs1099K = new() { Requested = true };
                break;
            default:
                return null;
        }

        if (!this.host.IsProduction())
        {
            string environment;

            if (this.host.IsDevelopment())
            {
                environment = $"{this.host.EnvironmentName}:{Environment.MachineName}";
            }
            else
            {
                environment = this.host.EnvironmentName;
            }

            create.Metadata.Add("environment", environment);
        }

        // Create a connected account.
        var service = new global::Stripe.AccountService(this.stripe);
        var account = await service.CreateAsync(create, null, cancellationToken);

        // Create domain entry.
        var method = new Cloudsume.Stripe.ReceivingMethod(Guid.NewGuid(), owner.Id, account.Id, DateTime.Now);

        await this.repository.CreateAsync(method);

        return method;
    }

    private async Task<PaymentReceivingMethod> ToDtoAsync(Cloudsume.Financial.ReceivingMethod domain, CancellationToken cancellationToken = default)
    {
        var status = await this.service.GetStatusAsync(domain, cancellationToken);

        return new(domain, status);
    }
}
