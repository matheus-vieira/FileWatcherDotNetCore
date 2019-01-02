namespace Watcher
{
    public interface IFileWatcher
    {
        System.Action Execute { get; set; }
    }
}
