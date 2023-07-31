namespace Cloudsume.Resume
{
    public sealed class TemplateNotFoundException : TemplateException
    {
        public TemplateNotFoundException(string name)
            : base($"No '{name}' template has been defined.")
        {
            this.Name = name;
        }

        public string Name { get; }
    }
}
