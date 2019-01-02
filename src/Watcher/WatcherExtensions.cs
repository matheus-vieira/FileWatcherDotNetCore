using Microsoft.Extensions.DependencyInjection;

namespace Watcher
{
    public static class WatcherExtensions
    {
        public static void AddFileWatcher(this IServiceCollection serviceCollection)
        {
            //serviceCollection.AddTransient<IFileWatcher, FileWatcher>();
        }
    }
}
