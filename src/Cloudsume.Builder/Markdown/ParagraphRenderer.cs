namespace Candidate.Server.Resume.Builder.Markdown
{
    using Markdig.Syntax;

    internal sealed class ParagraphRenderer : ObjectRenderer<ParagraphBlock>
    {
        protected override void Write(MarkdownRenderer renderer, ParagraphBlock obj)
        {
            renderer.WriteLeafInline(obj);

            if (!renderer.ImplicitParagraph)
            {
                renderer.Write(renderer.NewParagraph);
            }
        }
    }
}
