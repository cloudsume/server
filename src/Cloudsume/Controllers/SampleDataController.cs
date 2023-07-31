namespace Cloudsume.Controllers;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Candidate.Globalization;
using Cloudsume.DataOperations;
using Cloudsume.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Ultima.Extensions.Collections;
using Ultima.Extensions.Graphics;
using Domain = Cloudsume.Resume.SampleData;
using IDataManagerCollection = Cloudsume.Resume.IDataActionCollection<Cloudsume.IDataManager>;
using IJobRepository = Cloudsume.Data.IJobRepository;
using ISampleDataRepository = Cloudsume.Resume.ISampleDataRepository;
using IUserRepository = Cloudsume.Identity.IUserRepository;

[ApiController]
[Route("sample-data")]
public sealed class SampleDataController : ControllerBase
{
    private readonly ApplicationOptions options;
    private readonly ISampleDataRepository repository;
    private readonly IDataManagerCollection managers;
    private readonly ISampleOperationSerializer serializer;
    private readonly IUserRepository users;
    private readonly IJobRepository jobs;

    public SampleDataController(
        IOptions<ApplicationOptions> options,
        ISampleDataRepository repository,
        IDataManagerCollection managers,
        ISampleOperationSerializer serializer,
        IUserRepository users,
        IJobRepository jobs)
    {
        this.options = options.Value;
        this.repository = repository;
        this.managers = managers;
        this.serializer = serializer;
        this.users = users;
        this.jobs = jobs;
    }

    [HttpGet]
    [Authorize(AuthorizationPolicies.TemplateRead)]
    public async Task<IActionResult> ListAsync(CancellationToken cancellationToken = default)
    {
        var userId = this.users.GetId(this.User);
        var response = new List<SampleData>();

        foreach (var domain in await this.repository.ListAsync(userId, cancellationToken))
        {
            response.Add(this.ToDto(domain));
        }

        return this.Ok(response);
    }

    [HttpPatch("{jobId}/{cultureName}")]
    [Authorize(AuthorizationPolicies.TemplateWrite)]
    public async Task<IActionResult> UpdateAsync(
        [FromRoute] Guid jobId,
        [FromRoute] string cultureName,
        [FromForm] IFormCollection request,
        CancellationToken cancellationToken = default)
    {
        // Check if target job exists.
        if (jobId != Guid.Empty && !await this.jobs.IsExistsAsync(jobId, cancellationToken))
        {
            return this.NotFound();
        }

        // Map culture.
        var culture = this.GetCulture(cultureName);

        if (culture is null)
        {
            return this.NotFound();
        }

        // Check if culture valid.
        if (!this.options.AllowedTemplateCultures.Any(culture.ExistsInTree))
        {
            return this.NotFound();
        }

        // Load operations.
        var operations = await this.serializer.DeserializeAsync(jobId, culture, request, cancellationToken);

        if (operations == null)
        {
            return this.BadRequest(this.ModelState);
        }
        else if (!operations.Any())
        {
            // Don't allow empty body to reduce DOS attack.
            this.ModelState.AddModelError(string.Empty, "Invalid data.");
            return this.BadRequest(this.ModelState);
        }

        // Load current data.
        var userId = this.users.GetId(this.User);
        var current = new Dictionary<DataKey, List<Domain?>>();

        foreach (var data in await this.repository.ListAsync(userId, cancellationToken))
        {
            var set = current.GetOrAdd(new(data.TargetJob, data.Culture, data.Data.Type), () => new());

            set.Add(data);
        }

        // Execute operations. We can execute the operations in-place because the deserializer already ordered it for us.
        foreach (var operation in operations)
        {
            IActionResult? result = null;

            switch (operation)
            {
                case DeleteMultiplicableSampleData(var key, var type, var index):
                    result = await ExecuteMultiplicableDeleteAsync(key, type, index);
                    break;
                case DeleteSampleData(var key, var type):
                    result = await ExecuteDeleteAsync(key, type);
                    break;
                case UpdateMultiplicableSampleData(var key, var update, var index):
                    result = await ExecuteMultiplicableUpdateAsync(key, update, index);
                    break;
                case UpdateSampleData(var key, var update):
                    result = await ExecuteUpdateAsync(key, update);
                    break;
                default:
                    throw new NotImplementedException($"Operation '{operation.GetType()}' is not implemented.");
            }

            if (result is not null)
            {
                return result;
            }
        }

        // Create a response.
        var response = new List<SampleData>();

        foreach (var (key, updates) in current)
        {
            if (key.JobId != jobId || !key.Culture.Equals(culture))
            {
                continue;
            }

            foreach (var update in updates)
            {
                if (update is null)
                {
                    continue;
                }

                response.Add(this.ToDto(update));
            }
        }

        return this.Ok(response);

        async Task<IActionResult?> ExecuteMultiplicableDeleteAsync(string key, string type, int index)
        {
            // Get current data.
            if (!current.TryGetValue(new(jobId, culture, type), out var set) || index >= set.Count)
            {
                this.ModelState.TryAddModelError(key, "Invalid index.");
                return this.BadRequest(this.ModelState);
            }

            // Execute deletion.
            var target = set[index];

            if (target is not null)
            {
                if (((Candidate.Server.Resume.MultiplicativeData)target.Data).Id is not { } id)
                {
                    throw new Exception("Trying to delete an aggregated data. This should never happens.");
                }

                await this.repository.DeleteAsync(userId, jobId, culture, type, id);
                set[index] = null;
            }

            return null;
        }

        async Task<IActionResult?> ExecuteDeleteAsync(string key, string type)
        {
            // Get current data.
            if (!current.TryGetValue(new(jobId, culture, type), out var set))
            {
                this.ModelState.TryAddModelError(key, "Invalid data.");
                return this.BadRequest(this.ModelState);
            }

            // Execute deletion.
            await this.repository.DeleteAsync(userId, jobId, culture, type, null);
            set[0] = null;

            return null;
        }

        async Task<IActionResult?> ExecuteMultiplicableUpdateAsync(string key, Domain update, int index)
        {
            var data = (Candidate.Server.Resume.MultiplicativeData)update.Data;
            var type = data.Type;

            // Check if update causing circular reference.
            var jobTree = new HashSet<Guid>() { jobId };

            for (Domain? next = update; next is not null;)
            {
                // Check if data has parent job.
                if (next.ParentJob is not { } parentJob)
                {
                    break;
                }
                else if (!jobTree.Add(parentJob))
                {
                    this.ModelState.TryAddModelError(key, "The specified parent job causing circular reference.");
                    return this.BadRequest(this.ModelState);
                }

                // Get parent.
                if (!next.Data.HasFallbacks || ((Candidate.Server.Resume.MultiplicativeData)next.Data).BaseId is not { } baseId)
                {
                    break;
                }

                if (!current.TryGetValue(new(parentJob, culture, type), out var parents))
                {
                    break;
                }

                next = null;

                foreach (var parent in parents)
                {
                    if (parent is null)
                    {
                        throw new Exception("Data on non-target job has been deleted somehow.");
                    }
                    else if (((Candidate.Server.Resume.MultiplicativeData)parent.Data).Id is not { } parentId)
                    {
                        throw new Exception("Data identifier has been null somehow.");
                    }
                    else if (parentId == baseId)
                    {
                        next = parent;
                        break;
                    }
                }
            }

            // Get current data.
            var set = current.GetOrAdd(new(jobId, culture, type), () => new());

            // Check update type (create new VS update existing).
            if (data.Id is not { } id)
            {
                throw new Exception("No identifier is specified in the update.");
            }

            if (id == Guid.Empty)
            {
                // Check if we can create a data at target index.
                if (index < set.Count)
                {
                    if (set[index] is not null)
                    {
                        return this.Conflict();
                    }

                    set[index] = update;
                }
                else if (index == set.Count)
                {
                    if (set.Count >= this.managers[type].MaxLocal)
                    {
                        return this.Conflict();
                    }

                    set.Add(update);
                }
                else
                {
                    this.ModelState.TryAddModelError(key, "Invalid index.");
                    return this.BadRequest(this.ModelState);
                }

                // Generate new ID.
                data.Id = Guid.NewGuid();
            }
            else
            {
                // Check if data exists.
                var i = set.FindIndex(d => ((Candidate.Server.Resume.MultiplicativeData?)d?.Data)?.Id == id);

                if (i == -1)
                {
                    this.ModelState.TryAddModelError(key, "Invalid data identifier.");
                    return this.BadRequest(this.ModelState);
                }

                set[i] = update;
            }

            // Write the entry.
            await this.repository.WriteAsync(userId, update, index);

            return null;
        }

        async Task<IActionResult?> ExecuteUpdateAsync(string key, Domain update)
        {
            var data = update.Data;
            var type = data.Type;

            // Check if update causing circular reference.
            var jobTree = new HashSet<Guid>() { jobId };

            for (var next = update; ;)
            {
                // Check if data has parent job.
                if (next.ParentJob is not { } parentJob)
                {
                    break;
                }
                else if (!jobTree.Add(parentJob))
                {
                    this.ModelState.TryAddModelError(key, "The specified parent job causing circular reference.");
                    return this.BadRequest(this.ModelState);
                }

                // Get parent.
                if (!next.Data.HasFallbacks || !current.TryGetValue(new(parentJob, culture, type), out var parents))
                {
                    break;
                }

                var parent = parents.SingleOrDefault();

                if (parent is null)
                {
                    // The parents will never be an empty list.
                    throw new Exception("Data on non-target job has been deleted somehow.");
                }

                next = parent;
            }

            // Get current data.
            var set = current.GetOrAdd(new(jobId, culture, type), () => new());

            if (set.Count > 1)
            {
                throw new Exception("More than one data found on non-multiplicable data.");
            }

            // Write the entry.
            await this.repository.WriteAsync(userId, update, null);

            if (set.Count == 0)
            {
                set.Add(update);
            }
            else
            {
                set[0] = update;
            }

            return null;
        }
    }

    [HttpGet("{jobId}/{cultureName}/photo")]
    [Authorize(AuthorizationPolicies.TemplateRead)]
    public async Task<IActionResult> GetPhotoImageAsync([FromRoute] Guid jobId, [FromRoute] string cultureName, CancellationToken cancellationToken = default)
    {
        // Map culture.
        var culture = this.GetCulture(cultureName);

        if (culture is null)
        {
            return this.NotFound();
        }

        // Load photo.
        var userId = this.users.GetId(this.User);
        var domain = await this.repository.GetPhotoAsync(userId, jobId, culture, cancellationToken);

        if (domain is null)
        {
            return this.NotFound();
        }

        // Load image.
        var data = domain.Data;
        var info = data.Info.Value;

        if (info is null)
        {
            return this.NoContent();
        }

        var image = await data.GetImageAsync(cancellationToken);

        try
        {
            return this.File(image, info.Format.GetContentType(), false);
        }
        catch
        {
            await image.DisposeAsync();
            throw;
        }
    }

    private SampleData ToDto(Domain domain)
    {
        var data = domain.Data;
        var manager = this.managers[data.Type];
        var mappingServices = new DataMappingServices(this.Url, domain.TargetJob, this.GetCultureName(domain.Culture));

        return new(domain.TargetJob, domain.Culture.Name, manager.ToDto(data, mappingServices), domain.ParentJob);
    }

    private string GetCultureName(CultureInfo culture) => culture.Equals(CultureInfo.InvariantCulture) ? "default" : culture.Name;

    private CultureInfo? GetCulture(string name)
    {
        if (name.Length == 0)
        {
            return null;
        }
        else if (name.Equals("default", StringComparison.OrdinalIgnoreCase))
        {
            return CultureInfo.InvariantCulture;
        }
        else
        {
            try
            {
                return CultureInfo.GetCultureInfo(name);
            }
            catch (CultureNotFoundException)
            {
                return null;
            }
        }
    }

    private sealed class DataMappingServices : IDataMappingServices
    {
        private readonly IUrlHelper url;
        private readonly Guid job;
        private readonly string culture;

        public DataMappingServices(IUrlHelper url, Guid job, string culture)
        {
            this.url = url;
            this.job = job;
            this.culture = culture;
        }

        public string GetPhotoUrl(Candidate.Server.Resume.Data.PhotoInfo info)
        {
            var url = this.url.ActionLink("GetPhotoImage", values: new { jobId = this.job, cultureName = this.culture });

            if (url is null)
            {
                throw new Exception($"Failed to get a URL to {nameof(SampleDataController.GetPhotoImageAsync)}.");
            }

            return url;
        }
    }

    private sealed record DataKey(Guid JobId, CultureInfo Culture, string Type);
}
