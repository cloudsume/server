namespace Cloudsume.Resume;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Cornot;
using NetUlid;
using Ultima.Extensions.Currency;

public interface ITemplateRepository
{
    Task RegisterAsync(TemplateRegistration registration, CancellationToken cancellationToken = default);

    Task<IEnumerable<TemplateRegistration>> ListRegistrationAsync(CancellationToken cancellationToken = default);

    Task<IEnumerable<TemplateRegistration>> ListRegistrationByOwnerAsync(Guid ownerId, CancellationToken cancellationToken = default);

    Task<TemplateRegistration?> GetRegistrationAsync(Guid id, CancellationToken cancellationToken = default);

    Task<CultureInfo?> GetRegistrationCultureAsync(Guid id, CancellationToken cancellationToken = default);

    Task SetNameAsync(Guid id, string name, CancellationToken cancellationToken = default);

    Task SetDescriptionAsync(Guid id, string description, CancellationToken cancellationToken = default);

    Task SetWebsiteAsync(Guid id, Uri? website, CancellationToken cancellationToken = default);

    Task SetApplicableJobsAsync(Guid id, IEnumerable<Guid> jobs, CancellationToken cancellationToken = default);

    Task SetRegistrationCategoryAsync(Guid id, RegistrationCategory category, CancellationToken cancellationToken = default);

    Task SetPricesAsync(Guid id, IReadOnlyDictionary<CurrencyCode, decimal> prices, CancellationToken cancellationToken = default);

    Task SetUpdatedAsync(Guid id, DateTime time, CancellationToken cancellationToken = default);

    Task<int> CountRegistrationAsync(Guid ownerId, CancellationToken cancellationToken = default);

    Task CreateTemplateAsync(Template template, CancellationToken cancellationToken = default);

    Task<IEnumerable<Template>> ListTemplateAsync(Guid registrationId, CancellationToken cancellationToken = default);

    Task<Template?> GetTemplateAsync(Ulid id, CancellationToken cancellationToken = default);

    Task IncreaseResumeCountAsync(Template template, CancellationToken cancellationToken = default);

    Task DecreaseResumeCountAsync(Template template, CancellationToken cancellationToken = default);

    Task<int> CountTemplateAsync(Guid registrationId, CancellationToken cancellationToken = default);

    async Task<TemplateRegistration?> GetRegistrationAsync(Ulid templateId, CancellationToken cancellationToken = default)
    {
        var template = await this.GetTemplateAsync(templateId, cancellationToken);

        if (template == null)
        {
            return null;
        }

        var registration = await this.GetRegistrationAsync(template.RegistrationId, cancellationToken);

        if (registration == null)
        {
            throw new DataCorruptionException(template, "Unknown registration.");
        }

        return registration;
    }
}
