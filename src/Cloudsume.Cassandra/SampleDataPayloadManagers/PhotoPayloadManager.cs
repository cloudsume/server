namespace Cloudsume.Cassandra.SampleDataPayloadManagers;

using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Cloudsume.Resume;
using Cornot;
using Domain = Candidate.Server.Resume.Data.Photo;
using Row = Cloudsume.Cassandra.Models.IResumeSampleData;

internal sealed class PhotoPayloadManager : SampleDataPayloadManager<Domain>
{
    private readonly ISamplePhotoRepository repository;

    public PhotoPayloadManager(ISamplePhotoRepository repository)
    {
        this.repository = repository;
    }

    public override Task DeletePayloadAsync(Row row, CancellationToken cancellationToken = default)
    {
        return this.repository.DeleteAsync(row.Owner, row.TargetJob, CultureInfo.GetCultureInfo(row.Culture), cancellationToken);
    }

    public override Task ReadPayloadAsync(Domain data, Row row, CancellationToken cancellationToken = default)
    {
        if (data.Info.Value != null)
        {
            // Get the culture here so we will get an exception immediately if the value is not valid.
            var culture = CultureInfo.GetCultureInfo(row.Culture);

            data.Image = async cancellationToken =>
            {
                var image = await this.repository.ReadAsync(row.Owner, row.TargetJob, culture, cancellationToken);

                if (image == null)
                {
                    throw new DataCorruptionException(row, "No photo data is associated.");
                }

                return image;
            };
        }

        return Task.CompletedTask;
    }

    public override async Task WritePayloadAsync(Domain data, Row row, CancellationToken cancellationToken = default)
    {
        var owner = row.Owner;
        var targetJob = row.TargetJob;
        var culture = CultureInfo.GetCultureInfo(row.Culture);
        var info = data.Info.Value;

        if (info is null)
        {
            await this.repository.DeleteAsync(owner, targetJob, culture, cancellationToken);
        }
        else
        {
            await using var photo = await data.GetImageAsync(cancellationToken);

            await this.repository.WriteAsync(owner, targetJob, culture, photo, info.Size, cancellationToken);
        }
    }
}
