using System.IO;
using Microsoft.Extensions.Logging;

namespace UniqueWatcher
{
    public class UniqueWatcher : Watcher.FileWatcher
    {

        #region Dependency Injection

        private readonly ILogger<UniqueWatcher> _logger;
        private readonly UniqueWatcherSettings settings;

        #endregion Dependency Injection

        private readonly System.Collections.Generic.IList<string> filePathList = new System.Collections.Generic.List<string>();

        public UniqueWatcher(
            ILogger<UniqueWatcher> logger,
            Microsoft.Extensions.Options.IOptions<UniqueWatcherSettings> options
            ) : base(logger, options)
        {
            _logger = logger;
            settings = options.Value;
        }
        protected override void FileSystemWatcher_Created(object sender, FileSystemEventArgs e)
        {
            using (_logger.BeginScope(nameof(FileSystemWatcher_Created)))
            {
                _logger.LogInformation($"Adding {e.FullPath} to file list");
                filePathList.Add(e.FullPath);

                _logger.LogInformation($"File list count : {filePathList.Count} limit: {settings.Limit}");
                if (filePathList.Count < settings.Limit) return;

                //new Compress(filePathList, settings.ZipOutput)
                //    .Generate(settings.Directory);

                _logger.LogInformation($"Executing {this}");
            }
        }

        protected override void FileSystemWatcher_Deleted(object sender, FileSystemEventArgs e)
        {
            using (_logger.BeginScope(nameof(FileSystemWatcher_Deleted)))
            {
                _logger.LogInformation($"Removing {e.FullPath} from file list");
                filePathList.Remove(e.FullPath);
            }
        }
    }
}
