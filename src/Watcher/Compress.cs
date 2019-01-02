namespace Watcher
{
    public class Compress
    {
        private readonly System.Collections.Generic.IEnumerable<string> _filePathList;

        private readonly string _zipFileName = "result.zip";
        private readonly string _tempPath;

        public Compress(System.Collections.Generic.IEnumerable<string> list)
        {
            _filePathList = list;
            _tempPath = GenerateTempPath();
        }

        public Compress(System.Collections.Generic.IEnumerable<string> list, string zipName)
            : this(list)
        {
            if (string.IsNullOrWhiteSpace(zipName))
                throw new System.ArgumentNullException(nameof(zipName));

            _zipFileName = _zipFileName ?? zipName;
        }

        ~Compress()
        {

            System.IO.Directory.Delete(_tempPath, true);
        }

        public bool Generate(string originPath)
        {
            MoveFilesTemp(_tempPath, originPath);

            GenerateZipFile(originPath, _tempPath);

            return true;
        }

        private void GenerateZipFile(string originPath, string tempPath)
        {
            var zipFile = CombinePath(originPath, _zipFileName);
            System.IO.Compression.ZipFile.CreateFromDirectory(tempPath, zipFile);

            var target = CombinePath(originPath, "exit");

            CreateDirectory(target);

            target = CombinePath(target, _zipFileName);
            System.IO.File.Move(zipFile, target);
        }

        private void MoveFilesTemp(string tempPath, string originPath)
        {
            foreach (var file in _filePathList)
            {
                var srcFilePath = CombinePath(originPath, file);
                var tmpFilePath = CombinePath(tempPath, file);
                MoveFile(srcFilePath, tmpFilePath);
            }
        }

        private void MoveFile(string originFilePath, string destinationFilePath)
        {
            System.IO.File.Move(originFilePath, destinationFilePath);
        }

        private string GenerateTempPath()
        {
            var tempPath = CombinePath(System.IO.Path.GetTempPath(), System.Guid.NewGuid().ToString());
            CreateDirectory(tempPath);
            return tempPath;
        }

        private string CombinePath(string path, string otherPath)
        {
            return System.IO.Path.Combine(path, otherPath);
        }

        private void CreateDirectory(string directoryPath)
        {
            if (!System.IO.Directory.Exists(directoryPath))
                System.IO.Directory.CreateDirectory(directoryPath);
        }
    }
}
