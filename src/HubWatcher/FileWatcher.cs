namespace HubWatcher
{
    public class FileWatcher : Watcher.FileWatcher
    {
        public void Teste()
        {
            System.Console.WriteLine("Writing");
        }


        private readonly Microsoft.Extensions.Logging.ILogger<FileWatcher> _logger;
        //private readonly WatcherSettings settings;

        public FileWatcher(
            Microsoft.Extensions.Logging.ILogger<FileWatcher> logger
            //Microsoft.Extensions.Options.IOptions<UniqueWatcherSettings> options
            ) : base()
        {
            _logger = logger;
            Execute = Teste;
        }
    }
}
