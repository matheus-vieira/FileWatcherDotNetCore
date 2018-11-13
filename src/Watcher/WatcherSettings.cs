namespace Watcher
{
    public class WatcherSettings
    {
        public static string ConfigurationName = "Watcher";
        /// <summary>
        /// The directory to watch
        /// </summary>
        public string Directory { get; set; }
        /// <summary>
        /// The output path to zip file
        /// </summary>
        public string ZipOutput { get; set; }
        /// <summary>
        /// The limit of file to generate the zip file
        /// </summary>
        public int Limit { get; set; }
    }
}
