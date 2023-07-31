namespace NetCaptcha;

using System.Net;
using System.Threading;
using System.Threading.Tasks;
using NetCaptcha.ReCaptcha;

public static class ICaptchaServiceExtensions
{
    public static ValueTask<bool> ValidateReCaptchaAsync(
        this ICaptchaService service,
        string token,
        string action,
        IPAddress? clientAddress = null,
        double? minScore = null,
        CancellationToken cancellationToken = default)
    {
        var @params = new ReCaptchaValidationV3(token, action)
        {
            ClientAddress = clientAddress,
            MinScore = minScore,
        };

        return service.ValidateAsync(@params, cancellationToken);
    }
}
