namespace Microsoft.Extensions.DependencyInjection
{
    using Cloudsume.Financial;

    public static class IServiceCollectionExtensions
    {
        public static void AddFinancial(this IServiceCollection services)
        {
            services.AddSingleton<IReceivingMethodService, ReceivingMethodService>();
        }

        public static void AddReceivingMethodServiceAdaptor<T>(this IServiceCollection services) where T : class, IReceivingMethodServiceAdaptor
        {
            services.AddSingleton<IReceivingMethodServiceAdaptor, T>();
        }
    }
}
