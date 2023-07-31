namespace Ultima.Extensions.Telephony
{
    using System;
    using System.Globalization;
    using System.Text.Json.Serialization;
    using PhoneNumbers;

    [JsonConverter(typeof(TelephoneNumberJsonConverter))]
    public sealed class TelephoneNumber
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TelephoneNumber"/> class.
        /// </summary>
        /// <param name="country">
        /// The country where the number is belong to.
        /// </param>
        /// <param name="number">
        /// The number.
        /// </param>
        /// <exception cref="ArgumentException">
        /// <paramref name="number"/> is not a valid number for the country specified in <paramref name="country"/>.
        /// </exception>
        public TelephoneNumber(RegionInfo country, string number)
        {
            var numbering = PhoneNumberUtil.GetInstance();
            PhoneNumber parsed;

            try
            {
                parsed = numbering.Parse(number, country.Name);
            }
            catch (NumberParseException ex)
            {
                throw new ArgumentException("The value is not a valid phone number.", nameof(number), ex);
            }

            if (parsed.CountryCode != numbering.GetCountryCodeForRegion(country.Name))
            {
                throw new ArgumentException("The value is not a phone number of the specified country.", nameof(number));
            }

            this.Country = country;
            this.Number = number;
        }

        private TelephoneNumber(PhoneNumber number)
        {
            var numbering = PhoneNumberUtil.GetInstance();

            this.Country = new(numbering.GetRegionCodeForNumber(number));
            this.Number = number.NationalNumber.ToString(CultureInfo.InvariantCulture);
        }

        public RegionInfo Country { get; }

        public string Number { get; }

        public static TelephoneNumber GetExampleMobile(RegionInfo country)
        {
            var numbering = PhoneNumberUtil.GetInstance();
            var number = numbering.GetExampleNumberForType(country.Name, PhoneNumberType.MOBILE);

            if (number == null)
            {
                throw new NotImplementedException($"Country {country} is not implemented.");
            }

            return new(number);
        }

        public override string ToString() => this.ToString(CultureInfo.InvariantCulture);

        public string ToString(CultureInfo culture)
        {
            var numbering = PhoneNumberUtil.GetInstance();
            var number = numbering.Parse(this.Number, this.Country.Name);

            if (culture.Equals(CultureInfo.InvariantCulture))
            {
                return numbering.Format(number, PhoneNumberFormat.INTERNATIONAL);
            }

            var region = new RegionInfo(culture.Name);

            if (region.Name != this.Country.Name)
            {
                return numbering.Format(number, PhoneNumberFormat.INTERNATIONAL);
            }
            else
            {
                return numbering.Format(number, PhoneNumberFormat.NATIONAL);
            }
        }
    }
}
