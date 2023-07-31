namespace Cloudsume.Controllers;

using System;
using System.Threading;
using System.Threading.Tasks;
using Cloudsume.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("review-requests")]
public sealed class ReviewRequestsController : ControllerBase
{
    [HttpPost]
    [Authorize(AuthorizationPolicies.ReviewRequestWrite)]
    public Task<IActionResult> CreateAsync([FromBody] CreateReviewRequest request, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
