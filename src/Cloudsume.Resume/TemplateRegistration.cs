namespace Cloudsume.Resume;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Cloudsume.Template;
using Ultima.Extensions.Currency;

public sealed class TemplateRegistration
{
    public TemplateRegistration(
        Guid id,
        Guid userId,
        string name,
        string description,
        Uri? website,
        CultureInfo culture,
        IEnumerable<Guid> applicableJobs,
        RegistrationCategory category,
        IReadOnlyDictionary<CurrencyCode, decimal> prices,
        long resumeCount,
        UnlistedReason? unlistedReason,
        DateTime createdAt,
        DateTime updatedAt)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("The value is empty.", nameof(name));
        }

        if (website != null && (!website.IsAbsoluteUri || (website.Scheme != Uri.UriSchemeHttp && website.Scheme != Uri.UriSchemeHttps)))
        {
            throw new ArgumentException("The value is not supported.", nameof(website));
        }

        if (!applicableJobs.Any())
        {
            throw new ArgumentException("The value is empty.", nameof(applicableJobs));
        }

        if (!prices.Values.All(p => p > 0m))
        {
            throw new ArgumentException("The value contains price less than or equal zero.", nameof(prices));
        }

        if (resumeCount < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(resumeCount));
        }

        this.Id = id;
        this.UserId = userId;
        this.Name = name;
        this.Description = description;
        this.Website = website;
        this.Culture = culture;
        this.ApplicableJobs = applicableJobs;
        this.Category = category;
        this.Prices = prices;
        this.ResumeCount = resumeCount;
        this.UnlistedReason = unlistedReason;
        this.CreatedAt = createdAt;
        this.UpdatedAt = updatedAt;
    }

    public Guid Id { get; }

    /// <summary>
    /// Gets the owner of this registration.
    /// </summary>
    /// <value>
    /// The user identifier or <see cref="Guid.Empty"/> if the owner is system.
    /// </value>
    public Guid UserId { get; }

    public string Name { get; }

    public string Description { get; }

    public Uri? Website { get; }

    public CultureInfo Culture { get; }

    public IEnumerable<Guid> ApplicableJobs { get; }

    public RegistrationCategory Category { get; }

    public IReadOnlyDictionary<CurrencyCode, decimal> Prices { get; }

    public long ResumeCount { get; }

    public UnlistedReason? UnlistedReason { get; }

    public DateTime CreatedAt { get; }

    public DateTime UpdatedAt { get; }
}
