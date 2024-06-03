using System.IO;

namespace FtpExplorerWeb.Domain.Entities
{
    public abstract class FileInfoBase
    {
        private DirectoryInfo? _directory;
        private static DirectoryInfo _rootDirectory;
        protected FileInfoBase(DirectoryInfo? directory = null)
        {
            _directory = directory;
        }
        static FileInfoBase()
        {
            _rootDirectory = new DirectoryInfo(string.Empty, null);
        }
        public abstract string Name { get; }
        public DirectoryInfo Directory => _directory ?? _rootDirectory;
        public static DirectoryInfo RootDirectory => _rootDirectory;
        public string GetPath()
        {
            if (Directory == _rootDirectory)
            {
                return Name;
            }
            return Path.Combine(Directory.GetPath(), Name);
        }
    }
}
