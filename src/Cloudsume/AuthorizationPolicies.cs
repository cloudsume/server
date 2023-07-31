namespace Cloudsume;

public static class AuthorizationPolicies
{
    [AuthorizationPolicy("cloudsume.configuration.write")]
    public const string ConfigurationWrite = nameof(ConfigurationWrite);

    [AuthorizationPolicy("cloudsume.feedback.read")]
    public const string FeedbackRead = nameof(FeedbackRead);

    [AuthorizationPolicy("cloudsume.job.write")]
    public const string JobWrite = nameof(JobWrite);

    [AuthorizationPolicy("cloudsume.payment-receiving-method.read", AllowGuest = true)]
    public const string PaymentReceivingMethodRead = nameof(PaymentReceivingMethodRead);

    [AuthorizationPolicy("cloudsume.payment-receiving-method.write")]
    public const string PaymentReceivingMethodWrite = nameof(PaymentReceivingMethodWrite);

    [AuthorizationPolicy("cloudsume.resume.read", AllowGuest = true)]
    public const string ResumeRead = nameof(ResumeRead);

    [AuthorizationPolicy("cloudsume.resume.write", AllowGuest = true)]
    public const string ResumeWrite = nameof(ResumeWrite);

    [AuthorizationPolicy("cloudsume.review-request.read", AllowGuest = true)]
    public const string ReviewRequestRead = nameof(ReviewRequestRead);

    [AuthorizationPolicy("cloudsume.review-request.write", AllowGuest = true)]
    public const string ReviewRequestWrite = nameof(ReviewRequestWrite);

    [AuthorizationPolicy("cloudsume.template.read")]
    public const string TemplateRead = nameof(TemplateRead);

    [AuthorizationPolicy("cloudsume.template.write")]
    public const string TemplateWrite = nameof(TemplateWrite);

    [AuthorizationPolicy("cloudsume.template-license.read", AllowGuest = true)]
    public const string TemplateLicenseRead = nameof(TemplateLicenseRead);

    [AuthorizationPolicy("cloudsume.template-license.write", AllowGuest = true)]
    public const string TemplateLicenseWrite = nameof(TemplateLicenseWrite);
}
