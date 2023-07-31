namespace Cloudsume.Builder.Tests
{
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;
    using Candidate.Server.Resume;
    using Cloudsume.Resume;
    using Cloudsume.Services.Client;
    using Moq;
    using Ultima.Extensions.Collections;
    using Xunit;

    public sealed class ResumeBuilderTests
    {
        private static readonly CultureInfo UnitedStates = CultureInfo.GetCultureInfo("en-US");

        private readonly Mock<IDataActionCollection<IAttributeFactory>> attributes;
        private readonly KeyedByTypeCollection<TemplateRenderOptions> options;
        private readonly Mock<ILatexJobPoster> job;
        private readonly ResumeBuilder subject;

        public ResumeBuilderTests()
        {
            this.attributes = new();
            this.options = new();
            this.job = new();
            this.subject = new(this.attributes.Object);
        }

        [Theory]
        [InlineData("abc(data) ::= \"\"")]
        public async Task BuildAsync_NoResumeTemplate_ShouldThrow(string template)
        {
            await Assert.ThrowsAsync<TemplateNotFoundException>(() => this.subject.BuildAsync(template, UnitedStates, this.options, Enumerable.Empty<ResumeData>(), this.job.Object));
        }

        [Theory]
        [InlineData("resume(data) ::= <<", 1)]
        [InlineData("resume(data) ::= <<\nfoo bar\n>", 3)]
        public async Task BuildAsync_InvalidSyntax_ShouldThrow(string template, int line)
        {
            var ex = await Assert.ThrowsAsync<TemplateSyntaxException>(() => this.subject.BuildAsync(template, UnitedStates, this.options, Enumerable.Empty<ResumeData>(), this.job.Object));

            Assert.Equal(line, ex.Line);
        }
    }
}
