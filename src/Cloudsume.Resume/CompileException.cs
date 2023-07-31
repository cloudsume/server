namespace Cloudsume.Resume
{
    using System;

    public class CompileException : Exception
    {
        public CompileException(string message, string input)
            : base(message)
        {
            this.Input = input;
        }

        public CompileException(string message, string input, Exception? innerException)
            : base(message, innerException)
        {
            this.Input = input;
        }

        public string Input { get; }
    }
}
