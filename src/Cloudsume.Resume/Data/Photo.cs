namespace Candidate.Server.Resume.Data
{
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using Cloudsume.Resume;

    [ResumeData(StaticType)]
    public sealed class Photo : ResumeData
    {
        public const string StaticType = "photo";
        public const string InfoProperty = "photo";

        private ImageProvider? image;

        public Photo(DataProperty<PhotoInfo?> info, DateTime updatedAt)
            : base(updatedAt)
        {
            this.Info = info;
        }

        public delegate Task<Stream> ImageProvider(CancellationToken cancellationToken = default);

        public override string Type => StaticType;

        [DataProperty(InfoProperty)]
        public DataProperty<PhotoInfo?> Info { get; }

        public ImageProvider? Image
        {
            get => this.image;
            set
            {
                if (this.Info.Value == null && value != null)
                {
                    throw new InvalidOperationException("No info available.");
                }

                this.image = value;
            }
        }

        public Task<Stream> GetImageAsync(CancellationToken cancellationToken = default)
        {
            if (this.Image == null)
            {
                throw new InvalidOperationException("The instance contains no data.");
            }

            return this.Image(cancellationToken);
        }
    }
}
