namespace Cloudsume.Controllers;

using System;
using System.Threading;
using System.Threading.Tasks;
using Cloudsume.Analytics;
using Cloudsume.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using IConfigurationRepository = Cloudsume.Configurations.IConfigurationRepository;

[ApiController]
[Route("configurations")]
public sealed class ConfigurationsController : ControllerBase
{
    private readonly IConfigurationRepository repository;
    private readonly IStatsRepository stats;
    private readonly ILogger logger;

    public ConfigurationsController(IConfigurationRepository repository, IStatsRepository stats, ILogger<ConfigurationsController> logger)
    {
        this.repository = repository;
        this.stats = stats;
        this.logger = logger;
    }

    [HttpGet]
    public async Task<Configurations> GetAsync(CancellationToken cancellationToken = default)
    {
        var configurations = new Configurations()
        {
            #pragma warning disable CS0618
            SlackUri = await this.repository.GetSlackUriAsync(cancellationToken),
            #pragma warning restore CS0618
        };

        try
        {
            await this.stats.WriteConfigurationsLoadedAsync();
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Failed to write configurations loaded stats.");
        }

        return configurations;
    }

    [HttpPut("slack-uri")]
    [Authorize(AuthorizationPolicies.ConfigurationWrite)]
    public Task SetSlackUriAsync([FromBody] Uri? request, CancellationToken cancellationToken = default)
    {
        return this.repository.SetSlackUriAsync(request, cancellationToken);
    }

    [HttpGet("slack-uri")]
    public async Task<IActionResult> GetSlackUriAsync(CancellationToken cancellationToken = default)
    {
        var uri = await this.repository.GetSlackUriAsync(cancellationToken);

        if (uri == null)
        {
            return this.NoContent();
        }

        return this.Ok(uri);
    }
}
