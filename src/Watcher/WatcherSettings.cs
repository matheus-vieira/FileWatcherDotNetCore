namespace Watcher
{
    public class WatcherSettings
    {
        public static string ConfigFile { get; set; }
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
        /// <summary>
        ///  Indicates if subdirectories within the specified path should be monitored
        /// </summary>
        // Returns:
        //     true if you want to monitor subdirectories; otherwise, false. The default is
        //     false.
        public bool IncludeSubdirectories { get; set; } = false;
    }
}
