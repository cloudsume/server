namespace Candidate.Server.Resume.Builder.Markdown
{
    using Markdig.Syntax.Inlines;

    internal sealed class LiteralInlineRenderer : ObjectRenderer<LiteralInline>
    {
        protected override void Write(MarkdownRenderer renderer, LiteralInline obj)
        {
            renderer.WriteEscape(ref obj.Content);
        }
    }
}
