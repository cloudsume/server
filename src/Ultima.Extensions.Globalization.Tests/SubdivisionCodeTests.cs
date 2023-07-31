namespace Ultima.Extensions.Globalization.Tests
{
    using Xunit;

    public sealed class SubdivisionCodeTests
    {
        [Theory]
        [InlineData("TH", "10")]
        public void Parse_WithValidCode_ShouldReturnCorrespondingValue(string country, string subdivision)
        {
            var code = $"{country}-{subdivision}";
            var result = SubdivisionCode.Parse(code);

            Assert.Equal(country, result.Country);
            Assert.Equal(subdivision, result.Subdivision);
            Assert.Equal(code, result.Value);
        }
    }
}
