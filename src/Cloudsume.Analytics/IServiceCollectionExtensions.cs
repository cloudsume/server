namespace Microsoft.Extensions.DependencyInjection;

using Cloudsume.Analytics;

public static class IServiceCollectionExtensions
{
    public static void AddNullStatsRepository(this IServiceCollection services)
    {
        services.AddSingleton<IStatsRepository, NullStatsRepository>();
    }
}
