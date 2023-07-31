namespace Cloudsume.Resume.Tests;

using System;
using Xunit;

public sealed class AssetNameTests
{
    [Theory]
    [InlineData("main.stg")]
    [InlineData("images/abc.jpg")]
    [InlineData("fonts/foo/foo-italic.ttf")]
    public void Constructor_WithValidValue_ShouldNotThrow(string value)
    {
        var subject = new AssetName(value);

        Assert.Equal(value, subject.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData("/")]
    [InlineData("//")]
    [InlineData("f//b")]
    public void Constructor_WithInvalidValue_ShouldThrow(string value)
    {
        Assert.Throws<ArgumentException>(() => new AssetName(value));
    }
}
