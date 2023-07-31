namespace NetCaptcha.ReCaptcha;

public sealed class ReCaptchaValidationV3 : ReCaptchaValidation
{
    public ReCaptchaValidationV3(string token, string action)
        : base(token)
    {
        this.Action = action;
    }

    public string Action { get; }

    public double? MinScore { get; set; }
}
