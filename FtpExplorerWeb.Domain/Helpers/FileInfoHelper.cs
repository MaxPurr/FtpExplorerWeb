using FtpExplorerWeb.Domain.Entities;
using System;

namespace FtpExplorerWeb.Domain.Helpers
{
    public static class FileInfoHelper
    {
        private static char _extensionSeparator = '.';
        public static FileInfo GetFileInfoFromFullName(string fullName, DirectoryInfo directory)
        {
            if (!fullName.Contains(_extensionSeparator))
            {
                return new FileInfo(fullName, directory: directory);
            }
            var fileNameData = fullName.Split(_extensionSeparator);
            if (fileNameData.Length == 1)
            {
                string extension = fileNameData[1];
                return new FileInfo(string.Empty, extension, directory);
            }
            else if (fileNameData.Length == 2)
            {
                string nameOnly = fileNameData[0];
                string extension = fileNameData[1];
                return new FileInfo(nameOnly, extension, directory);
            }
            throw new ArgumentException("Wrong file name format");
        }
    }
}
