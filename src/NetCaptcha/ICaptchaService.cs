namespace NetCaptcha;

using System.Threading;
using System.Threading.Tasks;

public interface ICaptchaService
{
    ValueTask<bool> ValidateAsync(ValidationParams @params, CancellationToken cancellationToken = default);
}
