namespace Cloudsume.Resume
{
    using System;

    public sealed class TemplateSyntaxException : TemplateException
    {
        public TemplateSyntaxException(string message)
            : base(message)
        {
        }

        public TemplateSyntaxException(string message, int? line)
            : base(message)
        {
            this.Line = line;
        }

        public TemplateSyntaxException(string message, Exception? innerException)
            : base(message, innerException)
        {
        }

        public TemplateSyntaxException(string message, int? line, Exception? innerException)
            : base(message, innerException)
        {
            this.Line = line;
        }

        public int? Line { get; }
    }
}
