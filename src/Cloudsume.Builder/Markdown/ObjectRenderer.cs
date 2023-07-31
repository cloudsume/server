namespace Candidate.Server.Resume.Builder.Markdown
{
    using Markdig.Renderers;
    using Markdig.Syntax;

    internal abstract class ObjectRenderer<TObject> : MarkdownObjectRenderer<MarkdownRenderer, TObject>
        where TObject : MarkdownObject
    {
        protected ObjectRenderer()
        {
        }
    }
}
