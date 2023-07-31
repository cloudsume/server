namespace NetCaptcha.ReCaptcha;

using System.Net;

public class ReCaptchaValidation : ValidationParams
{
    public ReCaptchaValidation(string token)
    {
        this.Token = token;
    }

    public string Token { get; }

    public IPAddress? ClientAddress { get; set; }
}
