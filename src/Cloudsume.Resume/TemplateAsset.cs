namespace Cloudsume.Resume
{
    using System;

    public sealed class TemplateAsset
    {
        public TemplateAsset(AssetName name, int size, DateTime lastModified)
        {
            if (size < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(size));
            }

            this.Name = name;
            this.Size = size;
            this.LastModified = lastModified;
        }

        public AssetName Name { get; }

        public int Size { get; }

        public DateTime LastModified { get; }

        public override bool Equals(object? obj)
        {
            if (obj is not TemplateAsset other)
            {
                return false;
            }

            return other.Name == this.Name;
        }

        public override int GetHashCode()
        {
            return this.Name.GetHashCode();
        }

        public override string ToString()
        {
            return this.Name.Value;
        }
    }
}
