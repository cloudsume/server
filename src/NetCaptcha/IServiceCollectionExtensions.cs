namespace Microsoft.Extensions.DependencyInjection;

using NetCaptcha;

public static class IServiceCollectionExtensions
{
    public static ICaptchaServiceBuilder AddCaptcha(this IServiceCollection services)
    {
        services.AddSingleton<ICaptchaService, CaptchaService>();

        return new CaptchaServiceBuilder(services);
    }
}
