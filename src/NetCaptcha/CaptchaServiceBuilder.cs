namespace NetCaptcha;

using Microsoft.Extensions.DependencyInjection;

internal sealed class CaptchaServiceBuilder : ICaptchaServiceBuilder
{
    public CaptchaServiceBuilder(IServiceCollection services)
    {
        this.Services = services;
    }

    public IServiceCollection Services { get; }
}
