namespace Microsoft.Extensions.DependencyInjection;

using System;
using Cloudsume.Services.Client;
using Microsoft.Extensions.Configuration;

public static class IServiceCollectionExtensions
{
    public static void AddCloudsumeServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<ServicesClientOptions>().Bind(configuration);
        services.AddSingleton<IServicesClient, ServicesClient>();
    }

    public static void AddCloudsumeServices(this IServiceCollection services, Action<ServicesClientOptions> options)
    {
        services.Configure(options);
        services.AddSingleton<IServicesClient, ServicesClient>();
    }
}
