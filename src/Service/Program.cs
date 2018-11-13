using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Linq;
using Watcher;

namespace Service
{
    class Program
    {
        private static IConfiguration Configuration;

        private static async System.Threading.Tasks.Task Main(string[] args)
        {
            var builder = BuildHost(args);

            var isService = !(System.Diagnostics.Debugger.IsAttached || args.Contains("--console"));

            await builder.RunAsync(isService);
        }

        private static IHostBuilder BuildHost(string[] args) => new HostBuilder()
                .ConfigureAppConfiguration((hostContext, config) => ConfigureApp(config, args))
                .ConfigureServices((hostContext, services) => ConfigureServices(services));

        private static void ConfigureApp(IConfigurationBuilder configuration, string[] args)
        {
            configuration.AddEnvironmentVariables();
            configuration.AddJsonFile("appsettings.json", true, true);
            configuration.AddCommandLine(args);
            Configuration = configuration.Build();
        }

        private static void ConfigureServices(IServiceCollection serviceCollection)
        {
            // add configured instance of logging
            serviceCollection.AddSingleton(new Microsoft.Extensions.Logging.LoggerFactory()
              .AddConsole()
              .AddDebug());

            // add logging
            serviceCollection.AddLogging();

            serviceCollection.AddOptions();
            // Para o serviço do FileWatcher configure as dependências
            var section = Configuration.GetSection(WatcherSettings.ConfigurationName);
            serviceCollection.Configure<WatcherSettings>(section);
            serviceCollection.AddFileWatcher();

            // add services 
            //serviceCollection.AddTransient<ITestService, TestService>();
            // add app
            //serviceCollection.AddTransient<App>();
        }
    }
}
