namespace Cloudsume.Template;

using System;
using System.Collections.Generic;
using System.Linq;
using Ultima.Extensions.Currency;

public sealed class TemplatePack
{
    public TemplatePack(
        Guid id,
        Guid userId,
        string name,
        IReadOnlyDictionary<CurrencyCode, decimal> prices,
        IReadOnlySet<Guid> templates,
        DateTime createdAt,
        DateTime updatedAt)
    {
        if (prices.Any(e => e.Value <= 0m))
        {
            throw new ArgumentException("The value contains zero or negative price.", nameof(prices));
        }

        if (createdAt > updatedAt)
        {
            throw new ArgumentException($"The value is greater than {nameof(updatedAt)}.", nameof(createdAt));
        }

        this.Id = id;
        this.UserId = userId;
        this.Name = name;
        this.Prices = prices;
        this.Templates = templates;
        this.CreatedAt = createdAt;
        this.UpdatedAt = updatedAt;
    }

    public Guid Id { get; }

    /// <summary>
    /// Gets the owner of this pack.
    /// </summary>
    /// <value>
    /// The user identifier or <see cref="Guid.Empty"/> if the owner is a system.
    /// </value>
    public Guid UserId { get; }

    public string Name { get; }

    public IReadOnlyDictionary<CurrencyCode, decimal> Prices { get; }

    public IReadOnlySet<Guid> Templates { get; }

    public DateTime CreatedAt { get; }

    public DateTime UpdatedAt { get; }
}
