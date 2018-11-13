using System.IO;

namespace Watcher
{
    public class FileWatcher : Microsoft.Extensions.Hosting.IHostedService, System.IDisposable
    {
        private System.IO.FileSystemWatcher fileSystemWatcher;
        private readonly System.Collections.Generic.IList<string> filePathList = new System.Collections.Generic.List<string>();

        private readonly Microsoft.Extensions.Logging.ILogger<FileWatcher> _logger;
        private readonly WatcherSettings settings;

        public FileWatcher(
            Microsoft.Extensions.Logging.ILogger<FileWatcher> logger,
            Microsoft.Extensions.Options.IOptions<WatcherSettings> options
            )
        {
            _logger = logger;
            settings = options.Value;
        }

        private void FileSystemWatcher_Created(object sender, System.IO.FileSystemEventArgs e)
        {
            filePathList.Add(e.Name);

            if (filePathList.Count < settings.Limit) return;

            new Compress(filePathList, settings.ZipOutput)
                .Generate(settings.Directory);
        }

        private void FileSystemWatcher_Deleted(object sender, FileSystemEventArgs e)
        {
            filePathList.Remove(e.Name);
        }

        public System.Threading.Tasks.Task StartAsync(System.Threading.CancellationToken cancellationToken)
        {
            // instantiate the object
            fileSystemWatcher = new System.IO.FileSystemWatcher();
            // add a event listener to create files
            fileSystemWatcher.Created += FileSystemWatcher_Created;
            // add a event listener to deleted files
            fileSystemWatcher.Deleted += FileSystemWatcher_Deleted;
            // do not look subdirectories
            fileSystemWatcher.IncludeSubdirectories = false;
            // tell the watcher where to look
            fileSystemWatcher.Path = settings.Directory;
            // You must add this line - this allows events to fire.
            fileSystemWatcher.EnableRaisingEvents = true;

            return System.Threading.Tasks.Task.CompletedTask;
        }

        public System.Threading.Tasks.Task StopAsync(System.Threading.CancellationToken cancellationToken)
        {
            fileSystemWatcher.EnableRaisingEvents = false;

            return System.Threading.Tasks.Task.CompletedTask;
        }

        public void Dispose()
        {
            fileSystemWatcher?.Dispose();
        }
    }
}
