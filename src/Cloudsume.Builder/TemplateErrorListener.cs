namespace Cloudsume.Builder
{
    using System;
    using System.Collections.Generic;
    using NetTemplate;
    using NetTemplate.Misc;

    internal sealed class TemplateErrorListener : ITemplateErrorListener
    {
        private readonly List<TemplateMessage> errors;

        public TemplateErrorListener()
        {
            this.errors = new();
        }

        public IReadOnlyList<TemplateMessage> Errors => this.errors;

        public void CompiletimeError(TemplateMessage msg)
        {
            this.errors.Add(msg);
        }

        public void InternalError(TemplateMessage msg)
        {
            throw new NotImplementedException();
        }

        public void IOError(TemplateMessage msg)
        {
            this.errors.Add(msg);
        }

        public void RuntimeError(TemplateMessage msg)
        {
            if (msg.Error != ErrorType.NO_SUCH_PROPERTY)
            {
                this.errors.Add(msg);
            }
        }
    }
}
