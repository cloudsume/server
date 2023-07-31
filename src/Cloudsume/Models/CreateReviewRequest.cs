namespace Cloudsume.Models;

using System;

public sealed class CreateReviewRequest
{
    public CreateReviewRequest(Guid resumeId)
    {
        this.ResumeId = resumeId;
    }

    public Guid ResumeId { get; }
}
