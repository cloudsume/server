namespace Candidate.Globalization
{
    using System;

    public class TranslationNotFoundException : Exception
    {
        public TranslationNotFoundException()
        {
        }

        public TranslationNotFoundException(string? message)
            : base(message)
        {
        }

        public TranslationNotFoundException(string? message, Exception? innerException)
            : base(message, innerException)
        {
        }
    }
}
