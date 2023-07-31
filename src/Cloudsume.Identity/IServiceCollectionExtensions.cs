namespace Microsoft.Extensions.DependencyInjection
{
    using Cloudsume.Identity;

    public static class IServiceCollectionExtensions
    {
        public static void AddIdentity(this IServiceCollection services)
        {
            services.AddSingleton<IUserRepository, UserRepository>();
        }
    }
}
