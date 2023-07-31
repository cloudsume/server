namespace Cloudsume.Cassandra.ResumeDataMappers;

using System;
using Cloudsume.Cassandra.Models;
using Domain = Candidate.Server.Resume.Data.Photo;
using ImageFormat = Ultima.Extensions.Graphics.ImageFormat;
using PhotoInfo = Candidate.Server.Resume.Data.PhotoInfo;

internal sealed class PhotoMapper : ResumeDataMapper<Domain, PhotoData>
{
    public override PhotoData ToCassandra(Domain domain) => new()
    {
        Info = PhotoProperty.From(domain.Info, v => Photo.From(v)),
        UpdatedTime = domain.UpdatedAt,
    };

    public override Domain ToDomain(Guid id, Guid? parent, PhotoData cassandra)
    {
        var info = cassandra.Info.ToDomain(v => v is null ? null : new PhotoInfo((ImageFormat)v.Format, v.Size));

        return new(info, cassandra.UpdatedTime.LocalDateTime);
    }
}
