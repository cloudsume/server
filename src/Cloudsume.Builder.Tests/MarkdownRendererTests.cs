namespace Candidate.Server.Resume.Builder.Tests
{
    using System.Globalization;
    using Xunit;

    public sealed class MarkdownRendererTests
    {
        private static readonly CultureInfo English = CultureInfo.GetCultureInfo("en-US");
        private static readonly CultureInfo Thai = CultureInfo.GetCultureInfo("th-TH");

        [Fact]
        public void Render_WithEnglishUnorderedList_ShouldReturnCorrespondingLaTex()
        {
            // Arrange.
            var subject = new MarkdownRenderer(English);
            var markdown = @"Test.

- Item 1
- Item 2";

            // Act.
            var result = subject.Render(markdown);

            // Assert
            var expected = $"Test.{subject.NewParagraph}\\begin{{itemize}}\\item Item 1\\item Item 2\\end{{itemize}}";

            Assert.Equal(expected, result);
        }

        [Fact]
        public void Render_WithThaiUnorderedList_ShouldReturnCorrespondingLaTex()
        {
            // Arrange.
            var subject = new MarkdownRenderer(Thai);
            var markdown = @"Test ทดสอบ.

- ทดสอบ Test
- Item ทดสอบ Test";

            // Act.
            var result = subject.Render(markdown);

            // Assert
            var expected = $"Test \\textthai{{ทดสอบ}}.{subject.NewParagraph}\\begin{{itemize}}\\item \\textthai{{ทดสอบ}} Test\\item Item \\textthai{{ทดสอบ}} Test\\end{{itemize}}";

            Assert.Equal(expected, result);
        }

        [Fact]
        public void Render_WithEnglishOrderedList_ShouldReturnCorrespondingLaTex()
        {
            // Arrange.
            var subject = new MarkdownRenderer(English);
            var markdown = @"Test.

1. Item 1
2. Item 2

ABC.";

            // Act.
            var result = subject.Render(markdown);

            // Assert
            var expected = $"Test.{subject.NewParagraph}\\begin{{enumerate}}\\item Item 1\\item Item 2\\end{{enumerate}}ABC.{subject.NewParagraph}";

            Assert.Equal(expected, result);
        }

        [Fact]
        public void Render_WithThaiOrderedList_ShouldReturnCorrespondingLaTex()
        {
            // Arrange.
            var subject = new MarkdownRenderer(Thai);
            var markdown = @"Test ทดสอบ.

1. ทดสอบ Test
2. Item ทดสอบ Test

ไม่ทดสอบ";

            // Act.
            var result = subject.Render(markdown);

            // Assert
            var expected = $"Test \\textthai{{ทดสอบ}}.{subject.NewParagraph}\\begin{{enumerate}}\\item \\textthai{{ทดสอบ}} Test\\item Item \\textthai{{ทดสอบ}} Test\\end{{enumerate}}\\textthai{{ไม่ทดสอบ}}{subject.NewParagraph}";

            Assert.Equal(expected, result);
        }
    }
}
