namespace Cloudsume.DataManagers;

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Cloudsume.Models;
using Cloudsume.Server.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Ultima.Extensions.Graphics;
using Domain = Candidate.Server.Resume.Data.Photo;
using FromClient = Cloudsume.Resume.DataSources.FromClient;
using PhotoInfo = Candidate.Server.Resume.Data.PhotoInfo;
using Update = Cloudsume.Server.Models.DataProperty<string?>;

internal sealed class PhotoManager : UniqueDataManager<Domain, Update>
{
    public PhotoManager(IOptions<JsonOptions> json)
        : base(json)
    {
    }

    public override Task<SampleUpdate<Update>> ReadSampleUpdateAsync(Stream data, CancellationToken cancellationToken = default)
    {
        return this.ReadJsonAsync<SampleUpdate<Update>>(data, cancellationToken);
    }

    public override Task<Update> ReadUpdateAsync(Stream data, CancellationToken cancellationToken = default)
    {
        return this.ReadJsonAsync<Update>(data, cancellationToken);
    }

    public override Domain ToDomain(Update update, IReadOnlyDictionary<string, object> contents)
    {
        // Get image.
        IFormFile? content;

        if (update.Value == null)
        {
            return new(update.ToDomain<PhotoInfo>(v => null), DateTime.Now);
        }

        try
        {
            content = (IFormFile?)contents.GetValueOrDefault(update.Value);
        }
        catch (InvalidCastException ex)
        {
            throw new DataUpdateException("Invalid image.", ex);
        }

        if (content == null)
        {
            throw new DataUpdateException("Invalid image identifier.");
        }

        // Check image.
        switch (content.Length)
        {
            case 0:
                throw new DataUpdateException("Invalid image.");
            case > 1024 * 512:
                throw new DataUpdateException("The image is too large.");
        }

        var size = Convert.ToInt32(content.Length);
        PhotoInfo info;

        if (content.ContentType.Equals("image/jpeg", StringComparison.OrdinalIgnoreCase))
        {
            info = new(ImageFormat.JPEG, size);
        }
        else
        {
            throw new DataUpdateException("Invalid image.");
        }

        // ASP.NET Core is already enforced stream to have the same length as IFormFile.Length.
        // So we don't need to check on this.
        return new(new(info, new FromClient(), update.Flags), DateTime.Now)
        {
            Image = cancellationToken => Task.FromResult(content.OpenReadStream()),
        };
    }

    public override Update ToDto(Domain domain, IDataMappingServices services)
    {
        return DataProperty.From(domain.Info, info => info != null ? services.GetPhotoUrl(info) : null);
    }
}
