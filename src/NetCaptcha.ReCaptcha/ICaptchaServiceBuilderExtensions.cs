namespace Microsoft.Extensions.DependencyInjection;

using System;
using Microsoft.Extensions.Configuration;
using NetCaptcha.ReCaptcha;
using ICaptchaValidator = NetCaptcha.ICaptchaValidator;

public static class ICaptchaServiceBuilderExtensions
{
    public static ICaptchaServiceBuilder AddReCaptcha(this ICaptchaServiceBuilder builder, IConfiguration config)
    {
        return builder.AddReCaptcha(options => config.Bind(options));
    }

    public static ICaptchaServiceBuilder AddReCaptcha(this ICaptchaServiceBuilder builder, Action<ReCaptchaOptions> options)
    {
        builder.Services.Configure(options);
        builder.Services.AddSingleton<ICaptchaValidator, CaptchaValidator>();

        return builder;
    }
}
