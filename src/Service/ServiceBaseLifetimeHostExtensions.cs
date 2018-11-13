using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Service
{
    public static class ServiceBaseLifetimeHostExtensions
    {
        public static IHostBuilder UseServiceBaseLifetime(this IHostBuilder hostBuilder)
        {
            return hostBuilder.ConfigureServices((hostContext, services) =>
                services
                    .AddSingleton<IHostLifetime, ServiceBaseLifetime>());
        }

        public static System.Threading.Tasks.Task RunAsServiceAsync(this IHostBuilder hostBuilder, System.Threading.CancellationToken cancellationToken = default)
        {
            return hostBuilder.UseServiceBaseLifetime().Build().RunAsync(cancellationToken);
        }

        public static async System.Threading.Tasks.Task RunAsync(this IHostBuilder hostBuilder, bool isService, System.Threading.CancellationToken cancellationToken = default)
        {
            if (isService)
                await hostBuilder.RunAsServiceAsync();
            else
                await hostBuilder.RunConsoleAsync();
        }
    }

}
