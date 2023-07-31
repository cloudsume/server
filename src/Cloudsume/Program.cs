namespace Cloudsume;

using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Logging;

public static class Program
{
    public static void Main(string[] args)
    {
        IdentityModelEventSource.ShowPII = true;
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        string? env;

        // Common options.
        var builder = Host
            .CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(b => b.UseStartup<Startup>());

        // Load configurations from AWS Parameter Store.
        env = Environment.GetEnvironmentVariable("CLOUDSUME_AWS_CONFIG_PATH");

        if (env is not null)
        {
            builder.ConfigureAppConfiguration(b => b.AddSystemsManager(env));
        }

        return builder;
    }
}
