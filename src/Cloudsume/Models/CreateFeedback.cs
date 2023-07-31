namespace Cloudsume.Models;

using System.ComponentModel.DataAnnotations;
using System.Net.Mail;
using Domain = Cloudsume.Analytics.Feedback;

public sealed class CreateFeedback
{
    public CreateFeedback(int? score, string? detail, MailAddress? contact, string captchaToken)
    {
        this.Score = score;
        this.Detail = detail;
        this.Contact = contact;
        this.CaptchaToken = captchaToken;
    }

    [Range(Domain.MinScore, Domain.MaxScore)]
    public int? Score { get; }

    [MaxLength(10000)]
    public string? Detail { get; }

    public MailAddress? Contact { get; }

    [Required]
    public string CaptchaToken { get; }
}
