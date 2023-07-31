namespace Cloudsume.Models;

using System;
using System.Collections.Generic;
using System.Globalization;
using NetUlid;
using CurrencyCode = Ultima.Extensions.Currency.CurrencyCode;
using Domain = Cloudsume.Resume.TemplateRegistration;
using RegistrationCategory = Cloudsume.Resume.RegistrationCategory;

public sealed class TemplateRegistration
{
    public TemplateRegistration(Domain domain, IEnumerable<Guid> packs, Ulid? latestRelease, DateTime updatedAt)
    {
        this.Id = domain.Id;
        this.UserId = domain.UserId;
        this.Name = domain.Name;
        this.Description = domain.Description;
        this.Website = domain.Website;
        this.Language = domain.Culture;
        this.ApplicableJobs = domain.ApplicableJobs;
        this.Category = domain.Category;
        this.Prices = domain.Prices;
        this.Packs = packs;
        this.LatestRelease = latestRelease;
        this.ResumeCount = domain.ResumeCount;
        this.CreatedAt = domain.CreatedAt;
        this.UpdatedAt = updatedAt;
    }

    public Guid Id { get; }

    public Guid UserId { get; }

    public string Name { get; }

    public string Description { get; }

    public Uri? Website { get; }

    public CultureInfo Language { get; }

    public IEnumerable<Guid> ApplicableJobs { get; }

    public RegistrationCategory Category { get; }

    public IReadOnlyDictionary<CurrencyCode, decimal> Prices { get; }

    public IEnumerable<Guid> Packs { get; }

    public Ulid? LatestRelease { get; }

    public long ResumeCount { get; }

    public DateTime CreatedAt { get; }

    public DateTime UpdatedAt { get; }
}
