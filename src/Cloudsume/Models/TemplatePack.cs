namespace Cloudsume.Models;

using System;
using System.Collections.Generic;
using Ultima.Extensions.Currency;
using Domain = Cloudsume.Template.TemplatePack;

public sealed class TemplatePack
{
    public TemplatePack(Domain domain)
    {
        this.Id = domain.Id;
        this.Name = domain.Name;
        this.Prices = domain.Prices;
        this.Templates = domain.Templates;
    }

    public Guid Id { get; }

    public string Name { get; }

    public IReadOnlyDictionary<CurrencyCode, decimal> Prices { get; }

    public IEnumerable<Guid> Templates { get; }
}
