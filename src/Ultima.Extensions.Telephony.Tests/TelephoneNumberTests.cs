namespace Ultima.Extensions.Telephony.Tests
{
    using System;
    using System.Globalization;
    using Xunit;

    public sealed class TelephoneNumberTests
    {
        private static readonly RegionInfo Thailand = new("TH");

        [Fact]
        public void Constructor_WithValidNationalNumber_ShouldNotThrow()
        {
            var number = "027777777";
            var result = new TelephoneNumber(Thailand, number);

            Assert.Equal(Thailand, result.Country);
            Assert.Equal(number, result.Number);
        }

        [Theory]
        [InlineData("6627777777")]
        [InlineData("+6627777777")]
        public void Constructor_WithValidInternationalNumber_ShouldNotThrow(string number)
        {
            var result = new TelephoneNumber(Thailand, number);

            Assert.Equal(Thailand, result.Country);
            Assert.Equal(number, result.Number);
        }

        [Theory]
        [InlineData("+14082484450")]
        public void Constructor_WithNumberOfOtherCountry_ShouldThrow(string number)
        {
            Assert.Throws<ArgumentException>(() => new TelephoneNumber(Thailand, number));
        }
    }
}
