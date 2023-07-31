namespace Candidate.Server.Resume
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Threading;
    using System.Threading.Tasks;
    using Candidate.Server.Resume.Data;
    using Cloudsume.Resume;
    using NetUlid;

    public interface IResumeRepository
    {
        Task CreateAsync(Resume resume, CancellationToken cancellationToken = default);

        Task<Resume?> GetAsync(Guid userId, Guid id, CancellationToken cancellationToken = default);

        Task<int> CountAsync(Guid userId, CancellationToken cancellationToken = default);

        Task<IEnumerable<Resume>> ListAsync(Guid userId, CancellationToken cancellationToken = default);

        Task UpdateNameAsync(Guid userId, Guid id, string name, CancellationToken cancellationToken = default);

        Task UpdateTemplateAsync(Guid userId, Guid id, Ulid templateId, CancellationToken cancellationToken = default);

        Task SetRecruitmentConsentAsync(ResumeInfo resume, bool value, CancellationToken cancellationToken = default);

        Task SetUpdatedAsync(ResumeInfo resume, DateTime time, CancellationToken cancellationToken = default);

        /// <summary>
        /// Transfer all resusumes and its links from one user to another user.
        /// </summary>
        /// <param name="from">
        /// A user identifier to transfer from.
        /// </param>
        /// <param name="to">
        /// A user identifier to transfer to.
        /// </param>
        /// <param name="cancellationToken">
        /// The token to monitor for cancellation requests.
        /// </param>
        /// <returns>
        /// A list of resume identifier that transferred.
        /// </returns>
        /// <remarks>
        /// This method transfer only resumes. No any data will be transferred.
        /// </remarks>
        Task<IEnumerable<Guid>> TransferAsync(Guid from, Guid to, CancellationToken cancellationToken = default);

        Task<Photo?> GetPhotoAsync(Guid userId, Guid resumeId, CancellationToken cancellationToken = default);

        Task<IEnumerable<GlobalData<Photo>>> ListPhotoAsync(Guid userId, CultureInfo? culture, CancellationToken cancellationToken = default);

        Task<IEnumerable<ResumeData>> ListDataAsync(Guid userId, Guid resumeId, CancellationToken cancellationToken = default);

        Task<IEnumerable<GlobalData>> ListDataAsync(Guid userId, CultureInfo? culture, CancellationToken cancellationToken = default);

        Task<IEnumerable<ResumeData>> UpdateDataAsync(
            Resume resume,
            IEnumerable<LocalDataUpdate> updates,
            CancellationToken cancellationToken = default);

        Task UpdateDataAsync(Guid userId, CultureInfo culture, IEnumerable<ResumeData> data, CancellationToken cancellationToken = default);

        Task DeleteDataAsync(Resume resume, IEnumerable<LocalDataDelete> data, CancellationToken cancellationToken = default);

        Task<bool> TransferDataAsync(Guid from, Guid to, CancellationToken cancellationToken = default);

        Task DeleteAsync(Guid userId, Guid id, CancellationToken cancellationToken = default);
    }
}
