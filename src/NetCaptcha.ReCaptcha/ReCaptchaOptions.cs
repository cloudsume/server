namespace NetCaptcha.ReCaptcha;

public sealed class ReCaptchaOptions
{
    public string SecretKey { get; set; } = string.Empty;

    public double MinScore { get; set; } = 0.5;
}
