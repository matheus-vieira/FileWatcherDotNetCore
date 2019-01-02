using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Linq;

namespace Service
{
    public static class ServiceBaseLifetimeHostExtensions
    {
        public static IHostBuilder UseServiceBaseLifetime(this IHostBuilder hostBuilder)
        {
            return hostBuilder.ConfigureServices((hostContext, services) =>
                services.AddSingleton<IHostLifetime, ServiceBaseLifetime>());
        }

        public static System.Threading.Tasks.Task RunAsServiceAsync(this IHostBuilder hostBuilder, System.Threading.CancellationToken cancellationToken = default)
        {
            return hostBuilder.UseServiceBaseLifetime().Build().RunAsync(cancellationToken);
        }

        private static readonly string[] debgList = new[] { "--console", "-c", "--debug", "-d" };
        public static async System.Threading.Tasks.Task RunAsync(this ServiceHost host, string[] args, System.Threading.CancellationToken cancellationToken = default)
        {
            var isConsole = System.Diagnostics.Debugger.IsAttached ||
                host.Args.AsParallel().Any(el => debgList.AsParallel().Contains(el));

            if (isConsole) await host.RunConsoleAsync(cancellationToken);
            else await host.RunAsServiceAsync(cancellationToken);
        }
    }

}
