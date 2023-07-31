namespace Cloudsume.Controllers;

using System;
using System.Buffers;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Candidate.Server;
using Candidate.Server.Models;
using Cloudsume.Activities;
using Cloudsume.DataOperations;
using Cloudsume.Identity;
using Cloudsume.Models;
using Cloudsume.Server.Models;
using Cornot;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NetUlid;
using Ultima.Extensions.Graphics;
using Domain = Candidate.Server.Resume.Resume;
using IDataAggregator = Cloudsume.Resume.IDataAggregator;
using IDataManagerCollection = Cloudsume.Resume.IDataActionCollection<Cloudsume.IDataManager>;
using ILinkAccessRepository = Cloudsume.Resume.ILinkAccessRepository;
using ILinkDataCensor = Cloudsume.Resume.ILinkDataCensor;
using IResumeCompiler = Cloudsume.Resume.IResumeCompiler;
using IResumeLinkRepository = Cloudsume.Resume.IResumeLinkRepository;
using IResumeRepository = Candidate.Server.Resume.IResumeRepository;
using ITemplateAssetRepository = Cloudsume.Resume.ITemplateAssetRepository;
using ITemplateLicenseRepository = Cloudsume.Resume.ITemplateLicenseRepository;
using ITemplateRepository = Cloudsume.Resume.ITemplateRepository;
using IThumbnailRepository = Candidate.Server.Resume.IThumbnailRepository;
using IUserActivityRepository = Cloudsume.Analytics.IUserActivityRepository;
using LinkCensorship = Cloudsume.Resume.LinkCensorship;
using LinkId = Cloudsume.Resume.LinkId;
using MultiplicativeLocalDataUpdate = Candidate.Server.Resume.MultiplicativeLocalDataUpdate;
using ParentCollection = Cloudsume.Resume.ParentCollection;
using RegistrationCategory = Cloudsume.Resume.RegistrationCategory;
using TemplateLicenseStatus = Cloudsume.Template.TemplateLicenseStatus;

[ApiController]
[Route("resumes")]
public sealed class ResumesController : ControllerBase
{
    private readonly ApplicationOptions options;
    private readonly IAuthorizationService authorization;
    private readonly IResumeRepository repository;
    private readonly IDataManagerCollection managers;
    private readonly IResumeLinkRepository links;
    private readonly ILinkAccessRepository linkAccesses;
    private readonly IResumeCompiler compiler;
    private readonly IDataAggregator aggregator;
    private readonly ILinkDataCensor linkCensor;
    private readonly IUserRepository users;
    private readonly IResumeHelper resume;
    private readonly ITemplateRepository templates;
    private readonly ITemplateAssetRepository templateAssets;
    private readonly IThumbnailRepository thumbnails;
    private readonly IDataOperationSerializer dataSerializer;
    private readonly ITemplateLicenseRepository templateLicenses;
    private readonly IUserActivityRepository activities;
    private readonly ILogger logger;

    public ResumesController(
        IOptions<ApplicationOptions> options,
        IAuthorizationService authorization,
        IResumeRepository repository,
        IDataManagerCollection managers,
        IResumeLinkRepository links,
        ILinkAccessRepository linkAccesses,
        IResumeCompiler compiler,
        IDataAggregator aggregator,
        ILinkDataCensor linkCensor,
        IUserRepository users,
        IResumeHelper resume,
        ITemplateRepository templates,
        ITemplateAssetRepository templateAssets,
        IThumbnailRepository thumbnails,
        IDataOperationSerializer dataSerializer,
        ITemplateLicenseRepository templateLicenses,
        IUserActivityRepository activities,
        ILogger<ResumesController> logger)
    {
        this.options = options.Value;
        this.authorization = authorization;
        this.repository = repository;
        this.managers = managers;
        this.links = links;
        this.linkAccesses = linkAccesses;
        this.compiler = compiler;
        this.aggregator = aggregator;
        this.linkCensor = linkCensor;
        this.users = users;
        this.resume = resume;
        this.templates = templates;
        this.templateAssets = templateAssets;
        this.thumbnails = thumbnails;
        this.dataSerializer = dataSerializer;
        this.templateLicenses = templateLicenses;
        this.activities = activities;
        this.logger = logger;
    }

    [HttpPost]
    [Authorize(AuthorizationPolicies.ResumeWrite)]
    public async Task<IActionResult> CreateAsync([FromBody] CreateResume req, CancellationToken cancellationToken = default)
    {
        // Sanity check.
        if (req.Name == null)
        {
            throw new ArgumentException($"{nameof(req.Name)} is null.", nameof(req));
        }

        if (req.Template == null)
        {
            return this.BadRequest();
        }

        // Check if user already reached limit. It is possible to have race condition that allows user to have resumes exceed the limit.
        var userId = this.users.GetId(this.User);

        if (await this.repository.CountAsync(userId, cancellationToken) >= this.options.MaximumResumePerUser)
        {
            return this.Conflict();
        }

        // Create a domain model.
        var template = await this.GetTemplateAsync(req.Template.Value, cancellationToken);

        if (template == null)
        {
            return this.BadRequest();
        }

        var resume = new Domain(userId, Guid.NewGuid(), req.Name, template.Id, false, DateTime.Now, default);

        // Try to compile before create an entry in the database.
        var pages = await this.resume.UpdateThumbnailsAsync(resume, cancellationToken: cancellationToken);

        // If compilation is successful we can create an entry.
        await this.repository.CreateAsync(resume);
        await this.templates.IncreaseResumeCountAsync(template);

        // Write the activity.
        await this.activities.WriteAsync(new CreateResumeActivity(userId, template.Id, this.GetRemoteIp(), this.GetUserAgent()));

        return this.Ok(await this.ToDtoAsync(resume, template, pages, cancellationToken));
    }

    [HttpGet]
    public async Task<IActionResult> ListAsync([FromQuery(Name = "link")] LinkId? linkId, CancellationToken cancellationToken = default)
    {
        if (linkId is { } id)
        {
            // Get target link.
            var link = await this.links.FindAsync(id, cancellationToken);

            if (link is null)
            {
                return this.NoContent();
            }

            // Load resume.
            var resume = await this.repository.GetAsync(link.User, link.Resume, cancellationToken);

            if (resume is null)
            {
                throw new DataCorruptionException(link, "Invalid user or resume.");
            }

            // Load template and check if resume owner is allowed to get a PDF from the template.
            var template = await this.templates.GetTemplateAsync(resume.TemplateId, cancellationToken);

            if (template is null)
            {
                throw new DataCorruptionException(resume, $"{nameof(resume.TemplateId)} is not valid.");
            }

            if (!await this.CanDownloadAsync(resume, template, cancellationToken))
            {
                return this.NoContent();
            }

            // Load global data.
            var culture = await this.templates.GetRegistrationCultureAsync(template.RegistrationId, cancellationToken);

            if (culture is null)
            {
                throw new DataCorruptionException(template, $"{nameof(template.RegistrationId)} is not valid.");
            }

            var globals = await ParentCollection.LoadAsync(culture, async culture =>
            {
                var data = await this.repository.ListDataAsync(resume.UserId, culture, cancellationToken);

                return data.Select(d => d.Data);
            });

            // Combine data.
            var applicable = new HashSet<string>(template.ApplicableData);
            var data = this.aggregator.Aggregate(resume.Data.Where(d => applicable.Contains(d.Type)), globals);

            // Censor data.
            data = this.linkCensor.Run(data, link.Censorships, culture);

            // Compile resume.
            var assets = this.templateAssets.ReadAsync(template.Id, cancellationToken);
            await using var compiled = await this.compiler.CompileAsync(culture, assets, template.RenderOptions, data, cancellationToken);

            // Get download file name.
            var fileName = CultureInfo.CurrentCulture.Name switch
            {
                "en-US" => "Resume",
                "th-TH" => "เรซูเม",
                _ => throw new NotImplementedException($"Culture '{CultureInfo.CurrentCulture}' is not implemented."),
            };

            if (data.FirstOrDefault(d => d is Candidate.Server.Resume.Data.Name) is Candidate.Server.Resume.Data.Name name)
            {
                var first = name.FirstName.Value;
                var last = name.LastName.Value;

                switch (culture.Name)
                {
                    case "en-US":
                        if (first is not null && last is not null)
                        {
                            fileName = $"{first}-{last}-Resume";
                        }

                        break;
                    case "th-TH":
                        if (first is not null && last is not null)
                        {
                            fileName = $"{first} {last}";
                        }

                        break;
                    default:
                        throw new NotImplementedException($"Culture '{culture}' is not implemented.");
                }
            }

            // Log access.
            try
            {
                await this.linkAccesses.CreateAsync(link.Id, this.HttpContext.Connection.RemoteIpAddress, cancellationToken);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Failed to log access for link {Link}.", link.Id);
            }

            // Create response.
            compiled.LeaveOpen = true;

            try
            {
                return this.File(compiled.PDF, "application/pdf", $"{fileName}.pdf", false);
            }
            catch
            {
                compiled.LeaveOpen = false;
                throw;
            }
        }
        else
        {
            // Check if user is authenticated.
            var auth = await this.authorization.AuthorizeAsync(this.User, AuthorizationPolicies.ResumeRead);

            if (!auth.Succeeded)
            {
                return this.Forbid();
            }

            // List all resumes for current user.
            var userId = this.users.GetId(this.User);
            var domains = await this.repository.ListAsync(userId, cancellationToken);

            // Map to DTOs.
            var response = new List<ResumeSummary>();

            foreach (var domain in domains.OrderByDescending(r => r.UpdatedAt ?? DateTime.MinValue))
            {
                // Get a URL to thumbnail of the first page.
                var image = this.Url.ActionLink("GetThumbnail", values: new
                {
                    id = domain.Id,
                    page = 0,
                });

                if (image == null)
                {
                    throw new Exception($"Failed to get a URL to {this.GetThumbnailAsync} action.");
                }

                // Load links.
                var links = await this.LoadLinksAsync(domain.Id, cancellationToken);

                response.Add(new(domain, image, links));
            }

            // Write the activity.
            await this.activities.WriteAsync(new ListResumeActivity(userId, this.GetRemoteIp(), this.GetUserAgent()));

            return this.Ok(response);
        }
    }

    [HttpGet("{id}.{format?}")]
    [Authorize(AuthorizationPolicies.ResumeRead)]
    public async Task<IActionResult> GetByIdAsync([FromRoute] Guid id, [FromRoute] string? format = null, CancellationToken cancellationToken = default)
    {
        var userId = this.users.GetId(this.User);
        var resume = await this.repository.GetAsync(userId, id, cancellationToken);

        if (resume == null)
        {
            return this.NotFound();
        }

        if (format == "pdf")
        {
            var template = await this.templates.GetTemplateAsync(resume.TemplateId, cancellationToken);

            // Check if user allowed to get a PDF from selected template.
            if (!await this.CanDownloadAsync(resume, template, cancellationToken))
            {
                return this.NotFound();
            }

            // Compile.
            await using var compile = await this.resume.CompileAsync(resume, template, cancellationToken);

            compile.LeaveOpen = true;

            try
            {
                await this.activities.WriteAsync(new DownloadResumeActivity(userId, id, this.GetRemoteIp(), this.GetUserAgent()));

                return this.File(compile.PDF, "application/pdf", $"{resume.Name}.pdf", false);
            }
            catch
            {
                compile.LeaveOpen = false;
                throw;
            }
        }
        else if (format == null)
        {
            return this.Ok(await this.ToDtoAsync(resume));
        }
        else
        {
            return this.NotFound();
        }
    }

    [HttpDelete("{id}")]
    [Authorize(AuthorizationPolicies.ResumeWrite)]
    public async Task<IActionResult> DeleteByIdAsync([FromRoute] Guid id, CancellationToken cancellationToken = default)
    {
        // Load target resume and related data.
        var userId = this.users.GetId(this.User);
        var resume = await this.repository.GetAsync(userId, id, cancellationToken);

        if (resume == null)
        {
            return this.NotFound();
        }

        var template = await this.templates.GetTemplateAsync(resume.TemplateId, cancellationToken);

        if (template == null)
        {
            throw new DataCorruptionException(resume, $"{nameof(resume.TemplateId)} is not valid.");
        }

        var links = await this.links.ListAsync(resume.Id, cancellationToken);

        // Delete links.
        foreach (var link in links)
        {
            await this.links.DeleteAsync(resume.Id, link.Id);

            try
            {
                await this.linkAccesses.DeleteAsync(link.Id);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Failed to delete link accesses for link {Link}.", link.Id);
            }
        }

        // Delete resume.
        await this.repository.DeleteAsync(userId, resume.Id);

        // Delete thumbnail.
        try
        {
            await this.thumbnails.DeleteByResumeAsync(resume);
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Failed to delete thumbnail for resume {User}:{Resume}.", userId, resume.Id);
        }

        // Decrease template stats.
        try
        {
            await this.templates.DecreaseResumeCountAsync(template);
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Failed to decrease resume count for template {Template}.", template.Id);
        }

        // Write the activity.
        await this.activities.WriteAsync(new DeleteResumeActivity(userId, id, this.GetRemoteIp(), this.GetUserAgent()));

        return this.NoContent();
    }

    [HttpPut("{id}/name")]
    [Authorize(AuthorizationPolicies.ResumeWrite)]
    public async Task<IActionResult> UpdateNameAsync(
        [FromRoute] Guid id,
        [FromBody, Required, MaxLength(100)] string value,
        CancellationToken cancellationToken = default)
    {
        // Check if valid resume.
        var userId = this.users.GetId(this.User);
        var resume = await this.repository.GetAsync(userId, id, cancellationToken);

        if (resume is null)
        {
            return this.NotFound();
        }

        // Update name.
        await this.repository.UpdateNameAsync(userId, id, value, cancellationToken);
        await this.activities.WriteAsync(new RenameResumeActivity(userId, id, this.GetRemoteIp(), this.GetUserAgent()));

        return this.NoContent();
    }

    [HttpPut("{id}/recruitment-consent")]
    [Authorize(AuthorizationPolicies.ResumeWrite)]
    public async Task<IActionResult> UpdateRecruitmentConsentAsync([FromRoute] Guid id, [FromBody] bool value, CancellationToken cancellationToken = default)
    {
        // Load resume.
        var userId = this.users.GetId(this.User);
        var resume = await this.repository.GetAsync(userId, id, cancellationToken);

        if (resume is null)
        {
            return this.NotFound();
        }

        // Check if we need to update the consent.
        if (resume.RecruitmentConsent == value)
        {
            return this.BadRequest();
        }

        // Update consent.
        await this.repository.SetRecruitmentConsentAsync(resume, value, cancellationToken);

        return this.NoContent();
    }

    [HttpPut("{id}/template")]
    [Authorize(AuthorizationPolicies.ResumeWrite)]
    public async Task<IActionResult> UpdateTemplateAsync(
        [FromRoute] Guid id,
        [FromBody] Ulid value,
        CancellationToken cancellationToken = default)
    {
        // Check template.
        var template = await this.GetTemplateAsync(value, cancellationToken);

        if (template == null)
        {
            return this.BadRequest();
        }

        // Check template compatibilities.
        var userId = this.users.GetId(this.User);
        var resume = await this.repository.GetAsync(userId, id, cancellationToken);

        if (resume == null)
        {
            return this.NotFound();
        }
        else if (template.Id == resume.TemplateId)
        {
            return this.BadRequest();
        }

        var current = await this.templates.GetTemplateAsync(resume.TemplateId, cancellationToken);

        if (current == null)
        {
            throw new DataCorruptionException(resume, "Unknow template.");
        }

        var currentCulture = await this.templates.GetRegistrationCultureAsync(current.RegistrationId, cancellationToken);

        if (currentCulture == null)
        {
            throw new DataCorruptionException(current, "Unknow registration.");
        }

        var culture = await this.templates.GetRegistrationCultureAsync(template.RegistrationId, cancellationToken);

        if (culture == null)
        {
            throw new DataCorruptionException(template, "Unknown registration.");
        }

        if (!culture.Equals(currentCulture))
        {
            return this.BadRequest();
        }

        // Compile before update the data to ensure it is compilable.
        resume = resume with { TemplateId = template.Id };

        var pages = await this.resume.UpdateThumbnailsAsync(resume, cancellationToken: cancellationToken);

        // If compilation went well then we update template identifier.
        await this.repository.UpdateTemplateAsync(userId, id, resume.TemplateId);
        await this.templates.DecreaseResumeCountAsync(current);
        await this.templates.IncreaseResumeCountAsync(template);
        await this.activities.WriteAsync(new ChangeResumeTemplateActivity(userId, id, value, this.GetRemoteIp(), this.GetUserAgent()));

        return this.Ok(this.GetThumbnailUrls(resume.Id, pages));
    }

    [HttpPatch("{id}/data")]
    [Authorize(AuthorizationPolicies.ResumeWrite)]
    public async Task<IActionResult> UpdateDataAsync(
        [FromRoute] Guid id,
        [FromForm] IFormCollection form,
        CancellationToken cancellationToken = default)
    {
        // Load resume.
        var userId = this.users.GetId(this.User);
        var resume = await this.repository.GetAsync(userId, id, cancellationToken);

        if (resume == null)
        {
            return this.NotFound();
        }

        // Read data operations.
        var operations = await this.dataSerializer.DeserializeAsync(form, cancellationToken);

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

        // Execute deletions.
        var data = resume.Data.GroupBy(d => d.Type).ToDictionary(g => g.Key, g => g.ToList());
        var totals = data.ToDictionary(e => e.Key, e => e.Value.Count);
        var updates = new List<Candidate.Server.Resume.LocalDataUpdate>();
        var deletes = new List<Candidate.Server.Resume.LocalDataDelete>();
        List<Candidate.Server.Resume.ResumeData>? group;

        foreach (var operation in operations)
        {
            switch (operation)
            {
                case DeleteMultiplicableLocalData(var key, var type, var index):
                    if (!data.TryGetValue(type, out group) || group[0] is not Candidate.Server.Resume.MultiplicativeData || index >= group.Count)
                    {
                        this.ModelState.AddModelError(key, "Invalid deletion target.");
                        return this.BadRequest(this.ModelState);
                    }

                    deletes.Add(new(type, index));
                    group.RemoveAt(index);

                    if (group.Count == 0)
                    {
                        data.Remove(type);
                    }

                    totals[type] = group.Count;
                    break;
                case DeleteLocalData(var key, var type):
                    if (!data.TryGetValue(type, out group) || group[0] is Candidate.Server.Resume.MultiplicativeData)
                    {
                        this.ModelState.AddModelError(key, "Invalid deletion target.");
                        return this.BadRequest(this.ModelState);
                    }

                    deletes.Add(new(type));
                    data.Remove(type);
                    totals[type] = 0;
                    break;
                case UpdateMultiplicableLocalData(var key, var update, var index):
                    // Check if updates specify correct index.
                    if (!totals.TryGetValue(update.Type, out var total))
                    {
                        total = 0;
                    }

                    if (index > total)
                    {
                        this.ModelState.AddModelError(key, "Index out of range.");
                        return this.BadRequest(this.ModelState);
                    }
                    else if (index == total)
                    {
                        // Check limit when adding new entry.
                        if (index >= this.managers[update.Type].MaxLocal)
                        {
                            this.ModelState.AddModelError(key, "The data of this type is already reached limit.");
                            return this.BadRequest(this.ModelState);
                        }

                        totals[update.Type] = total + 1;
                    }

                    updates.Add(new Candidate.Server.Resume.MultiplicativeLocalDataUpdate(update, index));
                    break;
                case UpdateLocalData(_, var update):
                    updates.Add(new Candidate.Server.Resume.LocalDataUpdate(update));
                    break;
                default:
                    throw new NotImplementedException($"Operation {operation.GetType()} is not implemented.");
            }
        }

        if (deletes.Count > 0)
        {
            await this.repository.DeleteDataAsync(resume, deletes, cancellationToken);
        }

        resume = resume with { Data = data.SelectMany(e => e.Value).ToArray() };

        // Execute updates.
        resume = resume with
        {
            Data = await this.repository.UpdateDataAsync(resume, updates),
        };

        await this.repository.SetUpdatedAsync(resume, DateTime.Now);

        // Compile resume.
        var pages = await this.resume.UpdateThumbnailsAsync(resume);
        var thumbnails = this.GetThumbnailUrls(resume.Id, pages);
        var response = new DataUpdateResult(thumbnails, this.ToDto(resume.Data, resume.Id));

        // Write the activity.
        await this.activities.WriteAsync(new UpdateResumeActivity(
            userId,
            id,
            deletes.Select(d => (d.Type, d.Index)).ToHashSet(),
            updates.Select(u => u is MultiplicativeLocalDataUpdate m ? (m.Value.Type, m.Index) : (u.Value.Type, (int?)null)).ToHashSet(),
            pages,
            this.GetRemoteIp(),
            this.GetUserAgent()));

        return this.Ok(response);
    }

    [HttpGet("{id}/data/photo")]
    [Authorize(AuthorizationPolicies.ResumeRead)]
    public async Task<IActionResult> GetPhotoDataAsync([FromRoute] Guid id, CancellationToken cancellationToken = default)
    {
        var userId = this.users.GetId(this.User);
        var photo = await this.repository.GetPhotoAsync(userId, id, cancellationToken);

        if (photo == null)
        {
            return this.NotFound();
        }

        var info = photo.Info.Value;

        if (info == null)
        {
            return this.NoContent();
        }

        var content = await photo.GetImageAsync(cancellationToken);

        try
        {
            return this.File(content, info.Format.GetContentType(), false);
        }
        catch
        {
            await content.DisposeAsync();
            throw;
        }
    }

    [HttpGet("{id}/thumbnails/{page}")]
    [Authorize(AuthorizationPolicies.ResumeRead)]
    public async Task<IActionResult> GetThumbnailAsync([FromRoute] Guid id, [FromRoute] int page, CancellationToken cancellationToken = default)
    {
        // Load resume.
        var userId = this.users.GetId(this.User);
        var resume = await this.repository.GetAsync(userId, id, cancellationToken);

        if (resume == null)
        {
            return this.NotFound();
        }

        // Load thumbnail.
        var thumbnail = await this.thumbnails.FindByResumeAsync(resume, page, cancellationToken);

        if (thumbnail == null)
        {
            return this.NotFound();
        }

        try
        {
            return new ResumeThumbnailResult(thumbnail);
        }
        catch
        {
            await thumbnail.DisposeAsync();
            throw;
        }
    }

    [HttpPost("{id}/links")]
    [Authorize(AuthorizationPolicies.ResumeWrite)]
    public async Task<IActionResult> CreateLinkAsync(
        [FromRoute] Guid id,
        [FromBody] CreateResumeLink request,
        CancellationToken cancellationToken = default)
    {
        // Load target resume.
        var userId = this.users.GetId(this.User);
        var resume = await this.repository.GetAsync(userId, id, cancellationToken);

        if (resume == null)
        {
            return this.NotFound();
        }

        // We allowed only 10 links per resume.
        if (await this.links.CountAsync(resume.Id, cancellationToken) >= 10)
        {
            return this.Conflict();
        }

        // Create a link.
        var linkId = LinkId.Generate();
        var name = request.Name;
        var censorships = request.Censorships ?? new HashSet<LinkCensorship>();
        var link = new Cloudsume.Resume.Link(linkId, name, userId, resume.Id, censorships, DateTime.Now);

        await this.links.CreateAsync(link, cancellationToken);

        // Write the activity.
        await this.activities.WriteAsync(new CreateLinkActivity(userId, id, linkId, censorships, this.GetRemoteIp(), this.GetUserAgent()));

        return this.Ok(new ResumeLink(link, null));
    }

    [HttpDelete("{resumeId}/links/{linkId}")]
    [Authorize(AuthorizationPolicies.ResumeWrite)]
    public async Task<IActionResult> DeleteLinkAsync(
        [FromRoute] Guid resumeId,
        [FromRoute] LinkId linkId,
        CancellationToken cancellationToken = default)
    {
        // Load target resume.
        var userId = this.users.GetId(this.User);
        var resume = await this.repository.GetAsync(userId, resumeId, cancellationToken);

        if (resume == null)
        {
            return this.NotFound();
        }

        // Delete.
        await this.links.DeleteAsync(resumeId, linkId, cancellationToken);
        await this.activities.WriteAsync(new DeleteLinkActivity(userId, resumeId, linkId, this.GetRemoteIp(), this.GetUserAgent()));

        return this.NoContent();
    }

    [HttpGet("{resumeId}/links/{linkId}/accesses")]
    [Authorize(AuthorizationPolicies.ResumeRead)]
    public async Task<IActionResult> ListLinkAccessAsync(
        [FromRoute] Guid resumeId,
        [FromRoute] LinkId linkId,
        [FromQuery(Name = "skip_till")] Ulid? skipTill = null,
        CancellationToken cancellationToken = default)
    {
        // Check link ID.
        var userId = this.users.GetId(this.User);
        var link = await this.links.FindAsync(linkId, cancellationToken);

        if (link == null || link.User != userId || link.Resume != resumeId)
        {
            return this.NotFound();
        }

        // Load accesses.
        var accesses = await this.linkAccesses.ListAsync(link.Id, 100, skipTill, cancellationToken);

        await this.activities.WriteAsync(new ListLinkAccessActivity(userId, resumeId, linkId, skipTill, this.GetRemoteIp(), this.GetUserAgent()));

        return this.Ok(accesses.Select(a => new LinkAccess(a)).ToArray());
    }

    [HttpPut("{resumeId}/links/{linkId}/censorships")]
    [Authorize(AuthorizationPolicies.ResumeWrite)]
    public async Task<IActionResult> SetLinkCensorshipsAsync(
        [FromRoute] Guid resumeId,
        [FromRoute] LinkId linkId,
        [FromBody] HashSet<LinkCensorship> censorships,
        CancellationToken cancellationToken = default)
    {
        // Check target link.
        var userId = this.users.GetId(this.User);
        var link = await this.links.GetAsync(resumeId, linkId, cancellationToken);

        if (link is null || link.User != userId)
        {
            return this.NotFound();
        }

        // Update censorships.
        await this.links.SetCensorshipsAsync(link.Resume, link.Id, censorships, cancellationToken);
        await this.activities.WriteAsync(new UpdateLinkCensorshipsActivity(userId, resumeId, linkId, censorships, this.GetRemoteIp(), this.GetUserAgent()));

        return this.NoContent();
    }

    private async Task<Tuple<Domain, Stream>?> DownloadAsync(Guid userId, Guid resumeId, CancellationToken cancellationToken = default)
    {
        // Load resume.
        var resume = await this.repository.GetAsync(userId, resumeId, cancellationToken);

        if (resume is null)
        {
            throw new ArgumentException("The value is not a valid resume identifier.", nameof(resumeId));
        }

        var template = await this.templates.GetTemplateAsync(resume.TemplateId, cancellationToken);

        // Check if resume owner is allowed to get a PDF from selected template.
        if (!await this.CanDownloadAsync(resume, template, cancellationToken))
        {
            return null;
        }

        // Compile resume.
        await using var compile = await this.resume.CompileAsync(resume, template);

        compile.LeaveOpen = true;

        try
        {
            return Tuple.Create(resume, compile.PDF);
        }
        catch
        {
            compile.LeaveOpen = false;
            throw;
        }
    }

    private async Task<bool> CanDownloadAsync(
        Candidate.Server.Resume.Resume resume,
        Cloudsume.Resume.Template? template = null,
        CancellationToken cancellationToken = default)
    {
        // Load template.
        if (template == null)
        {
            template = await this.templates.GetTemplateAsync(resume.TemplateId, cancellationToken);

            if (template == null)
            {
                throw new DataCorruptionException(resume, "Unknown template.");
            }
        }

        // Check template category.
        if (template.Category == RegistrationCategory.Paid)
        {
            // Check if the owner of resume is the same as template.
            var registration = await this.templates.GetRegistrationAsync(template.RegistrationId, cancellationToken);

            if (registration == null)
            {
                throw new DataCorruptionException(template, "Unknown registration.");
            }
            else if (registration.UserId != resume.UserId)
            {
                // Check license.
                var license = await this.templateLicenses.GetAsync(resume.UserId, template.RegistrationId, cancellationToken);

                if (license == null || license.Status != TemplateLicenseStatus.Valid)
                {
                    return false;
                }
            }
        }

        return true;
    }

    /// <summary>
    /// Gets a template that can be using by the current user.
    /// </summary>
    /// <param name="id">
    /// Identifier of the template to get.
    /// </param>
    /// <param name="cancellationToken">
    /// The token to monitor for cancellation requests.
    /// </param>
    /// <returns>
    /// Template for <paramref name="id"/> or <see langword="null"/> if no such template.
    /// </returns>
    /// <remarks>
    /// Use this method only when user want to change the template. For other scenario use
    /// <see cref="ITemplateRepository.GetTemplateAsync(Ulid, CancellationToken)"/> instead.
    /// </remarks>
    private async Task<Cloudsume.Resume.Template?> GetTemplateAsync(Ulid id, CancellationToken cancellationToken = default)
    {
        var template = await this.templates.GetTemplateAsync(id, cancellationToken);

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

            var registration = await this.templates.GetRegistrationAsync(template.RegistrationId, cancellationToken);

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

    private async Task<Resume> ToDtoAsync(
        Domain domain,
        Cloudsume.Resume.Template? template = null,
        int pages = 0,
        CancellationToken cancellationToken = default)
    {
        // Load culture.
        if (template is null)
        {
            template = await this.templates.GetTemplateAsync(domain.TemplateId, cancellationToken);

            if (template is null)
            {
                throw new DataCorruptionException(domain, $"{nameof(domain.TemplateId)} is invalid.");
            }
        }

        var culture = await this.templates.GetRegistrationCultureAsync(template.RegistrationId, cancellationToken);

        if (culture is null)
        {
            throw new DataCorruptionException(template, $"{nameof(template.RegistrationId)} is invalid.");
        }

        // Load thumbnails.
        if (pages == 0)
        {
            pages = await this.thumbnails.GetPageCountAsync(domain, cancellationToken);
        }

        var thumbnails = this.GetThumbnailUrls(domain.Id, pages);
        var data = this.ToDto(domain.Data, domain.Id);
        var links = await this.LoadLinksAsync(domain.Id, cancellationToken);

        return new(domain, culture, thumbnails, data, links);
    }

    private IEnumerable<ResumeData> ToDto(IEnumerable<Candidate.Server.Resume.ResumeData> domains, Guid resumeId)
    {
        var services = new DataMappingServices(resumeId, this.Url);

        return domains.Select(d => this.managers[d.Type].ToDto(d, services)).ToArray();
    }

    private async Task<IEnumerable<ResumeLink>> LoadLinksAsync(Guid resumeId, CancellationToken cancellationToken = default)
    {
        var links = new List<ResumeLink>();

        foreach (var link in await this.links.ListAsync(resumeId, cancellationToken))
        {
            var access = await this.linkAccesses.GetLatestAsync(link.Id, cancellationToken);

            links.Add(new(link, access?.Id.Time));
        }

        return links;
    }

    private IEnumerable<string> GetThumbnailUrls(Guid resumeId, int pages)
    {
        var urls = new List<string>(pages);

        for (var i = 0; i < pages; i++)
        {
            var url = this.Url.ActionLink("GetThumbnail", values: new
            {
                id = resumeId,
                page = i,
            });

            if (url == null)
            {
                throw new Exception($"Failed to get a URL to {nameof(this.GetThumbnailAsync)}.");
            }

            urls.Add(url);
        }

        return urls;
    }

    private sealed class DataMappingServices : IDataMappingServices
    {
        private readonly Guid resumeId;
        private readonly IUrlHelper url;

        public DataMappingServices(Guid resumeId, IUrlHelper url)
        {
            this.resumeId = resumeId;
            this.url = url;
        }

        public string GetPhotoUrl(Candidate.Server.Resume.Data.PhotoInfo info)
        {
            var url = this.url.ActionLink("GetPhotoData", values: new { id = this.resumeId });

            if (url == null)
            {
                throw new Exception($"Failed to get a URL to {nameof(ResumesController.GetPhotoDataAsync)}.");
            }

            return url;
        }
    }
}
