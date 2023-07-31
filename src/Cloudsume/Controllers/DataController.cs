namespace Cloudsume.Controllers;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Candidate.Globalization;
using Cloudsume.Activities;
using Cloudsume.DataOperations;
using Cloudsume.Identity;
using Cloudsume.Models;
using Cornot;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NetUlid;
using Ultima.Extensions.Graphics;
using IDataManagerCollection = Cloudsume.Resume.IDataActionCollection<Cloudsume.IDataManager>;
using IResumeRepository = Candidate.Server.Resume.IResumeRepository;
using ITemplateRepository = Cloudsume.Resume.ITemplateRepository;
using IUserActivityRepository = Cloudsume.Analytics.IUserActivityRepository;
using ResumeComparer = Candidate.Server.Resume.ResumeComparer;

[ApiController]
[Route("data")]
public sealed class DataController : ControllerBase
{
    private readonly ApplicationOptions options;
    private readonly IUserRepository users;
    private readonly IResumeRepository repository;
    private readonly IDataManagerCollection managers;
    private readonly ITemplateRepository templates;
    private readonly IGlobalOperationSerializer serializer;
    private readonly IResumeHelper resume;
    private readonly IUserActivityRepository activities;

    public DataController(
        IOptions<ApplicationOptions> options,
        IUserRepository users,
        IResumeRepository repository,
        IDataManagerCollection managers,
        ITemplateRepository templates,
        IGlobalOperationSerializer serializer,
        IResumeHelper resume,
        IUserActivityRepository activities)
    {
        this.options = options.Value;
        this.users = users;
        this.repository = repository;
        this.managers = managers;
        this.templates = templates;
        this.serializer = serializer;
        this.resume = resume;
        this.activities = activities;
    }

    [HttpGet]
    [Authorize(AuthorizationPolicies.ResumeRead)]
    public async Task<IEnumerable<GlobalData>> ListAsync(CancellationToken cancellationToken = default)
    {
        var userId = this.users.GetId(this.User);
        var entries = await this.repository.ListDataAsync(userId, null, cancellationToken);

        return entries.Select(e => new GlobalData(e.Culture, this.managers[e.Data.Type].ToDto(e.Data, new DataMappingServices(e.Culture, this.Url)))).ToArray();
    }

    [HttpPatch("{language}")]
    [Authorize(AuthorizationPolicies.ResumeWrite)]
    public async Task<IActionResult> UpdateAsync(
        [FromRoute] string language,
        [FromForm] IFormCollection payload,
        CancellationToken cancellationToken = default)
    {
        // Check if language is allowed.
        var culture = this.GetCulture(language);

        if (culture == null || !this.options.AllowedTemplateCultures.Any(culture.ExistsInTree))
        {
            return this.NotFound();
        }

        var now = DateTime.Now;
        var userId = this.users.GetId(this.User);
        var resumes = await this.repository.ListAsync(userId, cancellationToken);
        var templates = new Dictionary<Ulid, (Cloudsume.Resume.Template Template, CultureInfo Culture)>();
        var cultures = new Dictionary<Guid, CultureInfo>();

        // Load all template's culture.
        foreach (var resume in resumes.Distinct(ResumeComparer.ByTemplate))
        {
            // TODO: Batch this and use the cheapest route as possible.
            var template = await this.templates.GetTemplateAsync(resume.TemplateId, cancellationToken);
            CultureInfo? registrationCulture;

            if (template == null)
            {
                throw new DataCorruptionException(resume, $"Unknow {nameof(resume.TemplateId)}.");
            }
            else if (cultures.TryGetValue(template.RegistrationId, out registrationCulture))
            {
                templates.Add(resume.TemplateId, (template, registrationCulture));
                continue;
            }

            registrationCulture = await this.templates.GetRegistrationCultureAsync(template.RegistrationId, cancellationToken);

            if (registrationCulture == null)
            {
                throw new DataCorruptionException(template, $"Unknow {nameof(template.RegistrationId)}.");
            }

            cultures.Add(template.RegistrationId, registrationCulture);
            templates.Add(resume.TemplateId, (template, registrationCulture));
        }

        // Deserialize payload.
        var operations = await this.serializer.DeserializeAsync(payload, cancellationToken);

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

        // Execute operations.
        var updates = new List<Candidate.Server.Resume.ResumeData>();
        var types = new HashSet<string>();
        var patched = (await this.repository.ListDataAsync(userId, culture, cancellationToken))
            .Select(e => e.Data)
            .GroupBy(d => d.Type)
            .ToDictionary(g => g.Key, g => g.ToList());

        foreach (var operation in operations)
        {
            switch (operation)
            {
                case DeleteMultiplicableGlobalData:
                case DeleteGlobalData:
                    return this.BadRequest(); // We don't support global deletion (yet).
                case UpdateGlobalData u:
                    var data = u.Update;

                    if (data is Candidate.Server.Resume.MultiplicativeData m)
                    {
                        if (!patched.TryGetValue(m.Type, out var set))
                        {
                            patched.Add(m.Type, set = new());
                        }

                        if (m.Id == Guid.Empty)
                        {
                            if (set.Count >= this.managers[m.Type].MaxGlobal)
                            {
                                return this.Conflict();
                            }

                            m.Id = Guid.NewGuid();

                            set.Add(m);
                        }
                        else
                        {
                            var i = set.FindIndex(d => ((Candidate.Server.Resume.MultiplicativeData)d).Id == m.Id);

                            if (i == -1)
                            {
                                return this.BadRequest();
                            }

                            set[i] = m;
                        }
                    }
                    else if (!patched.TryGetValue(data.Type, out var set))
                    {
                        patched.Add(data.Type, new() { data });
                    }
                    else
                    {
                        set[0] = data;
                    }

                    updates.Add(data);
                    types.Add(data.Type);
                    break;
                default:
                    throw new NotImplementedException($"Operation '{operation.GetType()}' is not implemented.");
            }
        }

        // Update database.
        var affectedResumes = new HashSet<Guid>();

        if (updates.Count > 0)
        {
            await this.repository.UpdateDataAsync(userId, culture, updates, cancellationToken);

            // Rebuild affected resumes.
            var globals = new Dictionary<CultureInfo, IReadOnlyDictionary<Resume.GlobalKey, Candidate.Server.Resume.ResumeData>>
            {
                { culture, updates.ToDictionary(d => new Resume.GlobalKey(d)) },
            };

            foreach (var resume in resumes)
            {
                // Check if language affected by updates.
                var (template, resumeCulture) = templates[resume.TemplateId];

                if (!culture.ExistsInTree(resumeCulture))
                {
                    continue;
                }

                // Check if data affected by updates.
                var affected = false;

                foreach (var data in resume.Data)
                {
                    if (types.Contains(data.Type) && await IsAffectedAsync(data, resumeCulture))
                    {
                        affected = true;
                        break;
                    }
                }

                if (affected)
                {
                    await this.resume.UpdateThumbnailsAsync(resume, template);
                    await this.repository.SetUpdatedAsync(resume, now);
                    affectedResumes.Add(resume.Id);
                }
            }

            async Task<bool> IsAffectedAsync(Candidate.Server.Resume.ResumeData data, CultureInfo parent)
            {
                if (!data.HasFallbacks)
                {
                    return false;
                }

                if (!globals.TryGetValue(parent, out var parents))
                {
                    parents = (await this.repository.ListDataAsync(userId, parent)).Select(e => e.Data).ToDictionary(d => new Resume.GlobalKey(d));
                    globals.Add(parent, parents);
                }

                Candidate.Server.Resume.ResumeData? p;

                if (data is Candidate.Server.Resume.MultiplicativeData m)
                {
                    if (m.BaseId == null || !parents.TryGetValue(new(m.Type, m.BaseId.Value), out p))
                    {
                        return false;
                    }
                }
                else
                {
                    if (!parents.TryGetValue(new(data.Type), out p))
                    {
                        return false;
                    }
                }

                if (parent.Equals(culture))
                {
                    return true;
                }
                else
                {
                    return await IsAffectedAsync(p, parent.Parent);
                }
            }
        }

        // Create response.
        var mappingServices = new DataMappingServices(culture, this.Url);
        var updatedData = updates.Select(d => this.managers[d.Type].ToDto(d, mappingServices)).ToArray();
        var response = new GlobalUpdateResult(updatedData, affectedResumes);

        // Write the activity.
        await this.activities.WriteAsync(new UpdateDataActivity(
            userId,
            culture,
            updates.Select(u => u is Candidate.Server.Resume.MultiplicativeData m ? (m.Type, m.Id) : (u.Type, null)).ToHashSet(),
            affectedResumes,
            this.GetRemoteIp(),
            this.GetUserAgent()));

        return this.Ok(response);
    }

    [HttpGet("{language}")]
    [Authorize(AuthorizationPolicies.ResumeRead)]
    public async Task<IActionResult> ListAsync([FromRoute] string language, CancellationToken cancellationToken = default)
    {
        var culture = this.GetCulture(language);

        if (culture == null)
        {
            return this.NotFound();
        }

        var userId = this.users.GetId(this.User);
        var entries = await this.repository.ListDataAsync(userId, culture, cancellationToken);
        var mappingServices = new DataMappingServices(culture, this.Url);
        var response = entries.Select(e => this.managers[e.Data.Type].ToDto(e.Data, mappingServices)).ToArray();

        return this.Ok(response);
    }

    [HttpGet("{language}/photo")]
    [Authorize(AuthorizationPolicies.ResumeRead)]
    public async Task<IActionResult> GetPhotoImageAsync([FromRoute] string language, CancellationToken cancellationToken = default)
    {
        var culture = this.GetCulture(language);

        if (culture == null)
        {
            return this.NotFound();
        }

        var userId = this.users.GetId(this.User);
        var photo = (await this.repository.ListPhotoAsync(userId, culture, cancellationToken)).SingleOrDefault()?.Data;

        if (photo == null)
        {
            return this.NotFound();
        }

        var info = photo.Info.Value;

        if (info == null)
        {
            return this.NoContent();
        }

        var image = await photo.GetImageAsync(cancellationToken);

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

    private CultureInfo? GetCulture(string language)
    {
        if (language.Length == 0)
        {
            return null;
        }
        else if (language.Equals("default", StringComparison.OrdinalIgnoreCase))
        {
            return CultureInfo.InvariantCulture;
        }
        else
        {
            try
            {
                return CultureInfo.GetCultureInfo(language);
            }
            catch (CultureNotFoundException)
            {
                return null;
            }
        }
    }

    private sealed class DataMappingServices : IDataMappingServices
    {
        private readonly CultureInfo culture;
        private readonly IUrlHelper url;

        public DataMappingServices(CultureInfo culture, IUrlHelper url)
        {
            this.culture = culture;
            this.url = url;
        }

        public string GetPhotoUrl(Candidate.Server.Resume.Data.PhotoInfo info)
        {
            var language = this.culture.Equals(CultureInfo.InvariantCulture) ? "default" : this.culture.Name;
            var url = this.url.ActionLink("GetPhotoImage", values: new { language });

            if (url == null)
            {
                throw new Exception($"Failed to get a URL to {nameof(DataController.GetPhotoImageAsync)}.");
            }

            return url;
        }
    }
}
