namespace Cloudsume.Resume.Tests;

using System;
using Cloudsume.Resume.Data;
using Cloudsume.Resume.DataSources;
using Moq;
using Ultima.Extensions.Globalization;
using Xunit;

public sealed class LayerSelectorTests
{
    [Fact]
    public void For_WithDisabledProperty_ThatPropertyShouldNotFallback()
    {
        // Arrange.
        var source1 = new Mock<IDataSource>();
        var source2 = new Mock<IDataSource>();
        var source3 = new Mock<IDataSource>();
        var source4 = new Mock<IDataSource>();
        var layer = new[]
        {
            new Address(new(null, source1.Object), new(null, source2.Object, PropertyFlags.Disabled), DateTime.MinValue),
            new Address(new(SubdivisionCode.Parse("TH-10"), source3.Object), new("Bang Bon", source4.Object), DateTime.MaxValue),
        };

        var subject = new LayerSelector<Address>(layer);

        // Act.
        var region = subject.For(d => d.Region);
        var street = subject.For(d => d.Street);

        // Assert.
        Assert.Equal(SubdivisionCode.Parse("TH-10"), region.Value);
        Assert.IsType<FromAggregator>(region.Source);
        Assert.Equal(PropertyFlags.None, region.Flags);

        Assert.Null(street.Value);
        Assert.IsType<FromAggregator>(street.Source);
        Assert.Equal(PropertyFlags.Disabled, street.Flags);

        Assert.Equal(DateTime.MaxValue, subject.LastUpdated);
    }
}
