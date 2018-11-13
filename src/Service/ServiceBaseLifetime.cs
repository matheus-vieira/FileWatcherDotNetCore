namespace Service
{
    public class ServiceBaseLifetime : System.ServiceProcess.ServiceBase, Microsoft.Extensions.Hosting.IHostLifetime
    {
        private readonly System.Threading.Tasks.TaskCompletionSource<object> _delayStart = new System.Threading.Tasks.TaskCompletionSource<object>();

        public ServiceBaseLifetime(Microsoft.Extensions.Hosting.IApplicationLifetime applicationLifetime)
        {
            ApplicationLifetime = applicationLifetime ?? throw new System.ArgumentNullException(nameof(applicationLifetime));
        }

        private Microsoft.Extensions.Hosting.IApplicationLifetime ApplicationLifetime { get; }

        public System.Threading.Tasks.Task WaitForStartAsync(System.Threading.CancellationToken cancellationToken)
        {
            cancellationToken.Register(() => _delayStart.TrySetCanceled());
            ApplicationLifetime.ApplicationStopping.Register(Stop);

            new System.Threading.Thread(Run).Start(); // Otherwise this would block and prevent IHost.StartAsync from finishing.
            return _delayStart.Task;
        }

        private void Run()
        {
            try
            {
                Run(this); // This blocks until the service is stopped.
                _delayStart.TrySetException(new System.InvalidOperationException("Stopped without starting"));
            }
            catch (System.Exception ex)
            {
                _delayStart.TrySetException(ex);
            }
        }

        public System.Threading.Tasks.Task StopAsync(System.Threading.CancellationToken cancellationToken)
        {
            Stop();
            return System.Threading.Tasks.Task.CompletedTask;
        }

        // Called by base.Run when the service is ready to start.
        protected override void OnStart(string[] args)
        {
            _delayStart.TrySetResult(null);
            base.OnStart(args);
        }

        // Called by base.Stop. This may be called multiple times by service Stop, ApplicationStopping, and StopAsync.
        // That's OK because StopApplication uses a CancellationTokenSource and prevents any recursion.
        protected override void OnStop()
        {
            ApplicationLifetime.StopApplication();
            base.OnStop();
        }
    }

}
