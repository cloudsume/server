namespace Candidate.Server.Tests
{
    using System;
    using Xunit;

    public sealed class RequireDefinedAttributeTests
    {
        private readonly RequireDefinedAttribute subject;

        public RequireDefinedAttributeTests()
        {
            this.subject = new RequireDefinedAttribute();
        }

        public enum TestEnum
        {
            V1,
            V2,
            V3,
        }

        [Flags]
        public enum TestFlag : uint
        {
            V2 = 0x00000001,
            V3 = 0x00000002,
            V4 = 0x80000000,
        }

        [Theory]
        [InlineData(TestEnum.V1, true)]
        [InlineData(TestEnum.V3, true)]
        [InlineData((TestEnum)100, false)]
        public void IsValid_WithNonNullableEnum_ShouldReturnCorrectResult(TestEnum value, bool expected)
        {
            var result = this.subject.IsValid(value);

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(null, true)]
        [InlineData(TestEnum.V2, true)]
        [InlineData((TestEnum)100, false)]
        public void IsValid_WithNullableEnum_ShouldReturnCorrectResult(TestEnum? value, bool expected)
        {
            var result = this.subject.IsValid(value);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void IsValid_WithValidFlags_ShouldReturnTrue()
        {
            var value = TestFlag.V2 | TestFlag.V4;
            var result = this.subject.IsValid(value);

            Assert.True(result);
        }

        [Fact]
        public void IsValid_WithInvalidFlags_ShouldReturnFalse()
        {
            var value = ((TestFlag)0x00000004) | TestFlag.V4;
            var result = this.subject.IsValid(value);

            Assert.False(result);
        }

        [Fact]
        public void IsValid_WithZeroFlags_ShouldReturnTrue()
        {
            var value = (TestFlag)0x00000000;
            var result = this.subject.IsValid(value);

            Assert.True(result);
        }
    }
}
