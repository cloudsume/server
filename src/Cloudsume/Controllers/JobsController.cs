namespace Cloudsume.Controllers;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudsume.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using IJobRepository = Cloudsume.Data.IJobRepository;

[ApiController]
[Route("jobs")]
public sealed class JobsController : ControllerBase
{
    private readonly IJobRepository repository;

    public JobsController(IJobRepository repository)
    {
        this.repository = repository;
    }

    [HttpPost]
    [Authorize(AuthorizationPolicies.JobWrite)]
    public async Task<Guid> CreateAsync([FromBody] CreateJob request, CancellationToken cancellationToken = default)
    {
        var job = new Cloudsume.Data.Job(Guid.NewGuid(), request.Names);

        await this.repository.CreateAsync(job, cancellationToken);

        return job.Id;
    }

    [HttpGet]
    public async Task<IEnumerable<Job>> ListAsync(CancellationToken cancellationToken = default)
    {
        var jobs = await this.repository.ListAsync(cancellationToken);

        return jobs.Select(j => new Job(j)).ToArray();
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAsync([FromRoute] Guid id, CancellationToken cancellationToken = default)
    {
        var job = await this.repository.GetAsync(id, cancellationToken);

        if (job == null)
        {
            return this.NotFound();
        }

        return this.Ok(new Job(job));
    }
}
