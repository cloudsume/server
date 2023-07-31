namespace Candidate.Server.Resume.Builder
{
    using System.Globalization;
    using System.IO;
    using Candidate.Server.Resume.Builder.Markdown;
    using Cloudsume.Builder;
    using Markdig.Helpers;
    using Markdig.Renderers;

    public sealed class MarkdownRenderer : TextRendererBase<MarkdownRenderer>
    {
        public MarkdownRenderer(CultureInfo culture)
            : base(new StringWriter() { NewLine = "\n" }) // StringWriter don't need to dispose.
        {
            this.Culture = culture;

            // block renderers
            this.ObjectRenderers.Add(new ListRenderer());
            this.ObjectRenderers.Add(new ParagraphRenderer());

            // inline renderers
            this.ObjectRenderers.Add(new LiteralInlineRenderer());
        }

        public CultureInfo Culture { get; }

        public string NewParagraph { get; set; } = "\n\n";

        public string? ListOptions { get; set; }

        public bool ImplicitParagraph { get; set; }

        public MarkdownRenderer WriteEscape(ref StringSlice slice)
        {
            if (slice.Start > slice.End)
            {
                return this;
            }

            return this.WriteEscape(slice.Text, slice.Start, slice.Length);
        }

        public MarkdownRenderer WriteEscape(string content, int offset, int length)
        {
            if (string.IsNullOrEmpty(content) || length == 0)
            {
                return this;
            }

            var latex = TexString.Transform(content.Substring(offset, length), this.Culture);

            return this.Write(latex);
        }

        public string Render(string markdown)
        {
            var writer = (StringWriter)this.Writer;

            Markdig.Markdown.Convert(markdown, this);
            writer.Flush();

            return writer.ToString();
        }
    }
}
