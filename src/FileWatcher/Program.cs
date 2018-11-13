namespace FileWatcher
{
    class Program
    {
        static System.IO.FileSystemWatcher fileSystemWatcher = new System.IO.FileSystemWatcher();

        static void Main(string[] args)
        {
            // instantiate the object
            fileSystemWatcher.Created += FileSystemWatcher_Created;

            // tell the watcher where to look
            // linux path
            //fileSystemWatcher.Path = @"/mnt/e/unique/teste/";
            // windows path
            fileSystemWatcher.Path = @"E:\unique\teste";

            // You must add this line - this allows events to fire.
            fileSystemWatcher.EnableRaisingEvents = true;

            System.Console.WriteLine("Listening...");
            System.Console.WriteLine("(Press any key to exit.)");

            System.Console.ReadLine();
        }

        private static readonly System.Collections.Generic.IList<string> filePathList = new System.Collections.Generic.List<string>();
        private static int FileLimit { get; set; } = 10;

        private static void FileSystemWatcher_Created(object sender, System.IO.FileSystemEventArgs e)
        {
            filePathList.Add(e.Name);

            if (filePathList.Count < FileLimit)
                return;

            Execute();
        }

        private static string _zipFileName = "result.zip";

        private static void Execute()
        {
            GenerateZipFile();
            filePathList.Clear();
        }

        private static void GenerateZipFile()
        {
            // gera zip com todos os arquivos
            System.Console.WriteLine("zipando arquivos...");

            var tempPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), System.Guid.NewGuid().ToString());
            var targetPath = fileSystemWatcher.Path;

            if (!System.IO.Directory.Exists(tempPath))
                System.IO.Directory.CreateDirectory(tempPath);

            foreach (var file in filePathList)
            {
                var sourceFilePath = System.IO.Path.Combine(targetPath, file);
                var destinationFilePath = System.IO.Path.Combine(tempPath, file);
                System.IO.File.Move(sourceFilePath, destinationFilePath);
            }

            var zipFile = System.IO.Path.Combine(fileSystemWatcher.Path, _zipFileName);
            System.IO.Compression.ZipFile.CreateFromDirectory(tempPath, zipFile);

            // mover arquivo zip
            System.Console.WriteLine("Movendo arquivo zip...");
            var target = System.IO.Path.Combine(fileSystemWatcher.Path, "exit");

            if (!System.IO.Directory.Exists(target))
                System.IO.Directory.CreateDirectory(target);

            target = System.IO.Path.Combine(target, _zipFileName);
            System.IO.File.Move(zipFile, target);
        }
    }
}
