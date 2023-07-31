namespace Cloudsume.Activities;

using System;
using System.Formats.Cbor;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using Cloudsume.Analytics;
using NetUlid;
using Ultima.Extensions.Currency;
using Ultima.Extensions.Primitives;

[Guid("a4038846-6ac1-466e-95e8-b2a1b889f40a")]
internal sealed class StartPurchaseTemplateActivity : UserActivity
{
    public StartPurchaseTemplateActivity(Guid userId, Guid templateId, CurrencyCode currency, decimal price, IPAddress ipAddress, string userAgent)
        : base(userId, ipAddress, userAgent)
    {
        if (price <= 0m)
        {
            throw new ArgumentOutOfRangeException(nameof(price));
        }

        this.TemplateId = templateId;
        this.Currency = currency;
        this.Price = price;
    }

    private StartPurchaseTemplateActivity(Guid userId, Ulid id, Guid templateId, CurrencyCode currency, decimal price, IPAddress ipAddress, string userAgent)
        : base(userId, id, ipAddress, userAgent)
    {
        if (price <= 0m)
        {
            throw new ArgumentOutOfRangeException(nameof(price));
        }

        this.TemplateId = templateId;
        this.Currency = currency;
        this.Price = price;
    }

    public Guid TemplateId { get; }

    public CurrencyCode Currency { get; }

    public decimal Price { get; }

    public static StartPurchaseTemplateActivity Deserialize(Guid userId, Ulid id, ReadOnlyMemory<byte> data, IPAddress ipAddress, string userAgent)
    {
        var reader = new CborReader(data, allowMultipleRootLevelValues: true);
        Guid templateId;
        CurrencyCode currency;
        decimal price;

        try
        {
            templateId = Uuid.ToGuid(reader.ReadDefiniteLengthByteString().Span);

            try
            {
                currency = CurrencyCode.Parse(Encoding.UTF8.GetString(reader.ReadDefiniteLengthTextStringBytes().Span));
            }
            catch (FormatException ex)
            {
                throw new ArgumentException($"Invalid {nameof(Currency)}.", nameof(data), ex);
            }

            price = reader.ReadDecimal();
        }
        catch (Exception ex) when (ex is InvalidOperationException || ex is CborContentException || ex is OverflowException)
        {
            throw new ArgumentException("Invalid data.", nameof(data), ex);
        }

        return new(userId, id, templateId, currency, price, ipAddress, userAgent);
    }

    public override byte[] Serialize()
    {
        var data = new CborWriter(allowMultipleRootLevelValues: true);

        data.WriteByteString(Uuid.FromGuid(this.TemplateId));
        data.WriteTextString(this.Currency.Value);
        data.WriteDecimal(this.Price);

        return data.Encode();
    }
}
