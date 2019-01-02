using Gelf.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reflection;
using UniqueWatcher;

namespace Service
{

    public class ServiceHost : Microsoft.Extensions.Hosting.HostBuilder
    {
        public readonly string[] Args;

        private IConfigurationRoot Configuration;

        public ServiceHost(string[] args)
        {
            Args = args;
            ConfigureHostConfiguration(configHost => ConfigureHost(configHost));
            ConfigureAppConfiguration((hostContext, config) => ConfigureApp(hostContext, config));
            ConfigureServices((hostContext, services) => ConfigureServices(hostContext, services));
        }


        private void ConfigureHost(IConfigurationBuilder configHost)
        {
            configHost.SetBasePath(System.IO.Directory.GetCurrentDirectory());
            configHost.AddJsonFile("hostsettings.json", optional: true);
            configHost.AddEnvironmentVariables(prefix: "PREFIX_");
            configHost.AddCommandLine(Args);
        }

        private void ConfigureApp(Microsoft.Extensions.Hosting.HostBuilderContext hostContext, IConfigurationBuilder configApp)
        {
            configApp.SetBasePath(System.IO.Directory.GetCurrentDirectory());
            configApp.AddJsonFile("appsettings.json", optional: true);
            configApp.AddJsonFile(
                path: $"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json",
                optional: true,
                reloadOnChange: true);
            configApp.AddJsonFile(UniqueWatcherSettings.ConfigFile);
            configApp.AddEnvironmentVariables(prefix: "PREFIX_");
            configApp.AddCommandLine(Args);

            Configuration = configApp.Build();

        }

        private void ConfigureServices(Microsoft.Extensions.Hosting.HostBuilderContext hostContext, IServiceCollection serviceCollection)
        {
            serviceCollection.AddOptions();

            // add logging
            serviceCollection.AddLogging(configLogging => ConfigureLogging(hostContext, configLogging));

            ConfigureUniqueWatcher(serviceCollection);
        }

        private void ConfigureUniqueWatcher(IServiceCollection serviceCollection)
        {

            IConfiguration section = Configuration.GetSection(UniqueWatcherSettings.ConfigurationName);
            serviceCollection.Configure<UniqueWatcherSettings>(section);
            serviceCollection.AddUniqueFileWatcher();
        }

        private void ConfigureLogging(Microsoft.Extensions.Hosting.HostBuilderContext hostContext, Microsoft.Extensions.Logging.ILoggingBuilder configLogging)
        {
            configLogging.AddConfiguration(Configuration.GetSection("Logging"));
            configLogging.AddConsole();
            configLogging.AddDebug();
            configLogging.AddEventSourceLogger();
            configLogging.AddEventLog();

            configLogging.AddGelf(options =>
            {
                // Optional customisation applied on top of settings in Logging:GELF configuration section.
                options.AdditionalFields["machine_name"] = System.Environment.MachineName;
                options.AdditionalFields["app_version"] = Assembly.GetEntryAssembly()
                    .GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
            });
        }
    }
}
