namespace Cloudsume.Resume
{
    using System;

    /// <summary>
    /// Represents errors that occur during transform main.stg.
    /// </summary>
    public class TemplateException : Exception
    {
        public TemplateException(string message)
            : base(message)
        {
        }

        public TemplateException(string message, Exception? innerException)
            : base(message, innerException)
        {
        }
    }
}
