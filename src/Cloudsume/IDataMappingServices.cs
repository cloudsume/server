namespace Cloudsume;

using PhotoInfo = Candidate.Server.Resume.Data.PhotoInfo;

public interface IDataMappingServices
{
    string GetPhotoUrl(PhotoInfo info);
}
