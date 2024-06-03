using FtpExplorerWeb.Domain.Helpers;
using System.Collections.Generic;

namespace FtpExplorerWeb.Domain.Entities
{
    public class DirectoryContent
    {
        private DirectoryInfo _directory;
        private List<FileInfo> _innerFiles;
        private List<DirectoryInfo> _innerDirectories;
        public DirectoryContent(DirectoryInfo directory)
        {
            _directory = directory;
            _innerFiles = new List<FileInfo>();
            _innerDirectories = new List<DirectoryInfo>();
        }
        public IReadOnlyCollection<FileInfo> Files => _innerFiles;
        public IReadOnlyCollection<DirectoryInfo> Directories => _innerDirectories;
        public FileInfo AddFile(string fullName)
        {
            var file = FileInfoHelper.GetFileInfoFromFullName(fullName, _directory);
            return AddFile(file);
        }
        public FileInfo AddFile(string fileName, string fileExtension)
        {
            var file = new FileInfo(fileName, fileExtension, _directory);
            return AddFile(file);
        }
        private FileInfo AddFile(FileInfo file)
        {
            _innerFiles.Add(file);
            return file;
        }
        public DirectoryInfo AddDirectory(string directoryName)
        {
            var directory = new DirectoryInfo(directoryName, _directory);
            return AddDirectory(directory);
        }
        private DirectoryInfo AddDirectory(DirectoryInfo directory)
        {
            _innerDirectories.Add(directory);
            return directory;
        }
        public void ClearFiles()
        {
            _innerFiles.Clear();
        }
        public void ClearDirectories()
        {
            _innerFiles.Clear();
        }
        public void ClearAll()
        {
            ClearFiles();
            ClearDirectories();
        }
    }
}
