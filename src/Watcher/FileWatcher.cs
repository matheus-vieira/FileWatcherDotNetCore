using Microsoft.Extensions.Logging;

namespace Watcher
{
    public class FileWatcher : Microsoft.Extensions.Hosting.IHostedService, System.IDisposable
    {

        private System.IO.FileSystemWatcher fileSystemWatcher;

        private readonly ILogger<FileWatcher> _logger;
        private readonly WatcherSettings settings;

        public FileWatcher(
            ILogger<FileWatcher> logger,
            Microsoft.Extensions.Options.IOptions<WatcherSettings> options
            )
        {
            if (options == null)
                throw new System.ArgumentNullException(nameof(options));

            _logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
            settings = options.Value;
        }

        protected virtual void FileSystemWatcher_Created(object sender, System.IO.FileSystemEventArgs e) => throw new System.NotImplementedException();

        protected virtual void FileSystemWatcher_Deleted(object sender, System.IO.FileSystemEventArgs e) => throw new System.NotImplementedException();

        public System.Threading.Tasks.Task StartAsync(System.Threading.CancellationToken cancellationToken)
        {
            if (fileSystemWatcher != null || settings == null)
                return System.Threading.Tasks.Task.CompletedTask;

            using (_logger.BeginScope(nameof(StartAsync)))
            {
                _logger.LogDebug($"instantiate the object {nameof(System.IO.FileSystemWatcher)}");
                // instantiate the object
                fileSystemWatcher = new System.IO.FileSystemWatcher();
                _logger.LogDebug($"add a event listener to create files");
                // add a event listener to create files
                fileSystemWatcher.Created += FileSystemWatcher_Created;
                _logger.LogDebug($"add a event listener to deleted files");
                // add a event listener to deleted files
                fileSystemWatcher.Deleted += FileSystemWatcher_Deleted;
                // do not look subdirectories
                _logger.LogDebug($"IncludeSubdirectories {settings.IncludeSubdirectories}");
                fileSystemWatcher.IncludeSubdirectories = settings.IncludeSubdirectories;
                // tell the watcher where to look
                _logger.LogDebug($"Setting the path {settings.Directory}");
                fileSystemWatcher.Path = settings.Directory;
                // You must add this line - this allows events to fire.
                _logger.LogDebug($"Enabling events");
                fileSystemWatcher.EnableRaisingEvents = true;

                return System.Threading.Tasks.Task.CompletedTask;
            }
        }

        public System.Threading.Tasks.Task StopAsync(System.Threading.CancellationToken cancellationToken)
        {
            using (_logger?.BeginScope(nameof(StopAsync)))
            {
                _logger?.LogDebug($"Disabling events");
                if (fileSystemWatcher != null)
                    fileSystemWatcher.EnableRaisingEvents = false;

                return System.Threading.Tasks.Task.CompletedTask;
            }
        }

        public void Dispose()
        {
            using (_logger?.BeginScope(nameof(Dispose)))
            {
                _logger?.LogDebug($"Trying to dispose");
                fileSystemWatcher?.Dispose();
            }
        }
    }
}
