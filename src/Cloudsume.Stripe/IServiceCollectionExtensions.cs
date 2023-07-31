namespace Microsoft.Extensions.DependencyInjection
{
    using System;
    using Cloudsume.Stripe;
    using Microsoft.Extensions.Options;
    using Stripe;

    public static class IServiceCollectionExtensions
    {
        public static void AddStripe(this IServiceCollection services, Action<StripeOptions> options)
        {
            // Foundation.
            services.AddOptions<StripeOptions>().Configure(options).ValidateDataAnnotations();
            services.AddSingleton<IStripeClient>(p => new StripeClient(p.GetRequiredService<IOptions<StripeOptions>>().Value.Key));

            // Application services.
            services.AddReceivingMethodServiceAdaptor<ReceivingMethodServiceAdaptor>();
        }
    }
}
