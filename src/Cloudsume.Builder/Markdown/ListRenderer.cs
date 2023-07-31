namespace Candidate.Server.Resume.Builder.Markdown
{
    using Markdig.Syntax;

    internal sealed class ListRenderer : ObjectRenderer<ListBlock>
    {
        protected override void Write(MarkdownRenderer renderer, ListBlock obj)
        {
            var options = renderer.ListOptions;

            if (obj.IsOrdered)
            {
                if (options != null)
                {
                    renderer.Write(@$"\begin{{enumerate}}[{options}]");
                }
                else
                {
                    renderer.Write(@"\begin{enumerate}");
                }
            }
            else if (options != null)
            {
                renderer.Write(@$"\begin{{itemize}}[{options}]");
            }
            else
            {
                renderer.Write(@"\begin{itemize}");
            }

            foreach (ListItemBlock item in obj)
            {
                var implicitParagraph = renderer.ImplicitParagraph;

                renderer.ImplicitParagraph = true;

                renderer.Write(@"\item ");
                renderer.WriteChildren(item);

                renderer.ImplicitParagraph = implicitParagraph;
            }

            if (obj.IsOrdered)
            {
                renderer.Write(@"\end{enumerate}");
            }
            else
            {
                renderer.Write(@"\end{itemize}");
            }
        }
    }
}
