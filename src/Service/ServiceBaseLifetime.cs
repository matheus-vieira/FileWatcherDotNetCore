using Microsoft.Extensions.Logging;

namespace Service
{
    public class ServiceBaseLifetime : System.ServiceProcess.ServiceBase, Microsoft.Extensions.Hosting.IHostLifetime
    {
        private readonly System.Threading.Tasks.TaskCompletionSource<object> _delayStart = new System.Threading.Tasks.TaskCompletionSource<object>();
        private readonly Microsoft.Extensions.Logging.ILogger<ServiceBaseLifetime> _logger;

        public ServiceBaseLifetime(
            Microsoft.Extensions.Logging.ILogger<ServiceBaseLifetime> logger,
            Microsoft.Extensions.Hosting.IApplicationLifetime applicationLifetime)
        {
            ApplicationLifetime = applicationLifetime ?? throw new System.ArgumentNullException(nameof(applicationLifetime));
            _logger = logger;
        }

        private Microsoft.Extensions.Hosting.IApplicationLifetime ApplicationLifetime { get; }

        public System.Threading.Tasks.Task WaitForStartAsync(System.Threading.CancellationToken cancellationToken)
        {
            using (_logger.BeginScope(nameof(WaitForStartAsync)))
            {
                _logger.LogInformation("Registering cancelation token");
                cancellationToken.Register(() => _delayStart.TrySetCanceled());

                _logger.LogInformation($"Registering {nameof(ApplicationLifetime.ApplicationStopping)}");
                ApplicationLifetime.ApplicationStopping.Register(Stop);

                _logger.LogInformation($"Starting {nameof(Run)}");
                new System.Threading.Thread(Run).Start(); // Otherwise this would block and prevent IHost.StartAsync from finishing.
                return _delayStart.Task;
            }
        }

        private void Run()
        {
            using (_logger.BeginScope(nameof(Run)))
            {
                try
                {
                    _logger.LogInformation("Running");
                    Run(this); // This blocks until the service is stopped.
                    _logger.LogError("Stopped without starting");
                    _delayStart.TrySetException(new System.InvalidOperationException("Stopped without starting"));
                }
                catch (System.Exception ex)
                {
                    _delayStart.TrySetException(ex);
                }
            }
        }

        public System.Threading.Tasks.Task StopAsync(System.Threading.CancellationToken cancellationToken)
        {
            using (_logger.BeginScope(nameof(StopAsync)))
            {
                _logger.LogInformation("Stopping");
                Stop();
                _logger.LogInformation("Stopped");
                return System.Threading.Tasks.Task.CompletedTask;
            }
        }

        // Called by base.Run when the service is ready to start.
        protected override void OnStart(string[] args)
        {
            using (_logger.BeginScope(nameof(OnStart)))
            {
                _logger.LogInformation("Starting");
                _delayStart.TrySetResult(null);
                base.OnStart(args);
                _logger.LogInformation("Started");
            }
        }

        // Called by base.Stop. This may be called multiple times by service Stop, ApplicationStopping, and StopAsync.
        // That's OK because StopApplication uses a CancellationTokenSource and prevents any recursion.
        protected override void OnStop()
        {
            using (_logger.BeginScope(nameof(OnStop)))
            {
                _logger.LogInformation("Stopping");
                ApplicationLifetime.StopApplication();
                base.OnStop();
                _logger.LogInformation("Stopped");
            }
        }
    }

}
