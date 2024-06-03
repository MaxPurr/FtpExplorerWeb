using FtpExplorerWeb.Domain.Entities;
using System;

namespace FtpExplorerWeb.Domain.Extensions
{
    public static class DirectoryContentExtensions
    {
        private static int _fileNameStartIndex = 62;
        public static void FillFromFileData(this DirectoryContent content, string fileData)
        {
            if (fileData.Length <= _fileNameStartIndex)
            {
                throw new ArgumentException("Wrong file data format");
            }
            string fullFileName = fileData.Substring(_fileNameStartIndex);
            if (fileData[0] == 'd')
            {
                content.AddDirectory(fullFileName);
            }
            else
            {
                content.AddFile(fullFileName);
            }
        }
    }
}
