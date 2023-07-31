namespace Cloudsume.Cassandra.ResumeDataPayloadManagers;

using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Candidate.Server.Resume.Data;
using Candidate.Server.Resume.Data.Repositories;
using Cloudsume.Server.Cassandra;
using Cornot;

internal sealed class PhotoPayloadManager : UniqueResumeDataPayloadManager<Photo>
{
    private readonly IPhotoImageRepository repository;

    public PhotoPayloadManager(IPhotoImageRepository repository)
    {
        this.repository = repository;
    }

    public override Task DeletePayloadAsync(Guid userId, Guid resumeId, CancellationToken cancellationToken = default)
    {
        return this.repository.DeleteAsync(userId, resumeId, cancellationToken);
    }

    public override Task DeletePayloadAsync(Guid userId, CultureInfo culture, CancellationToken cancellationToken = default)
    {
        return this.repository.DeleteAsync(userId, culture, cancellationToken);
    }

    public override Task ReadPayloadAsync(Photo data, IResumeData row, CancellationToken cancellationToken = default)
    {
        var info = data.Info.Value;

        if (info != null)
        {
            data.Image = async cancellationToken =>
            {
                object key = row.ResumeId != Guid.Empty ? row.ResumeId : CultureInfo.GetCultureInfo(row.Language);
                var image = await this.repository.GetAsync(row.UserId, key, cancellationToken);

                if (image == null)
                {
                    throw new DataCorruptionException(row, "No image data is associated.");
                }

                return image;
            };
        }

        return Task.CompletedTask;
    }

    public override async Task TransferPayloadAsync(IResumeData row, Guid to, CancellationToken cancellationToken)
    {
        // Check if row have image data.
        var data = (Cloudsume.Cassandra.Models.PhotoData?)row.Data;
        var info = data?.Info?.Value;

        if (info == null)
        {
            return;
        }

        // Move image data.
        object key = row.ResumeId != Guid.Empty ? row.ResumeId : CultureInfo.GetCultureInfo(row.Language);
        var image = await this.repository.GetAsync(row.UserId, key, cancellationToken);

        if (image == null)
        {
            throw new DataCorruptionException(row, "No image data is associated.");
        }

        await using (image)
        {
            await this.repository.UpdateAsync(to, key, image, info.Size, cancellationToken);
        }

        await this.repository.DeleteAsync(row.UserId, key);
    }

    public override async Task UpdatePayloadAsync(Photo data, IResumeData row, CancellationToken cancellationToken = default)
    {
        var userId = row.UserId;
        object key = row.ResumeId != Guid.Empty ? row.ResumeId : CultureInfo.GetCultureInfo(row.Language);

        if (data.Info.Value is { } info)
        {
            await using var image = await data.GetImageAsync(cancellationToken);
            await this.repository.UpdateAsync(userId, key, image, info.Size, cancellationToken);
        }
        else
        {
            await this.repository.DeleteAsync(userId, key, cancellationToken);
        }
    }
}
