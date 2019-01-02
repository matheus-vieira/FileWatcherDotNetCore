using Microsoft.Extensions.DependencyInjection;

namespace UniqueWatcher
{
    public static class UniqueWatcherExtensions
    {
        public static void AddUniqueFileWatcher(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddHostedService<UniqueWatcher>();
        }
    }
}
