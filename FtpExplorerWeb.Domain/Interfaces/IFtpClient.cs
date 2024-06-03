using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using FtpExplorerWeb.Domain.Entities;
using Stream = System.IO.Stream;

namespace FtpExplorerWeb.Domain.Interfaces
{
    public interface IFtpClient
    {
        Task<FileInfo> UploadFileAsync(DirectoryInfo destinationDirectory, string sourseFileName, Stream sourseStream, CancellationToken token);
        Task<Stream> GetFileDownloadStreamAsync(FileInfo sourseFile, CancellationToken token);
        Task<DirectoryInfo> MakeDirectoryAsync(DirectoryInfo destinationDirectory, string soursedirectoryName, CancellationToken token);
        Task DeleteFileAsync(FileInfo file, CancellationToken token);
        Task DeleteFilesAsync(IEnumerable<FileInfo> files, CancellationToken token);
        Task RemoveDirectoryAsync(DirectoryInfo directory, CancellationToken token);
        Task RemoveDirectoriesAsync(IEnumerable<DirectoryInfo> directories, CancellationToken token);
        Task RemoveFilesAndDirectoriesAsync(IEnumerable<FileInfo> files, IEnumerable<DirectoryInfo> directories, CancellationToken token);
        Task ClearDirectoryAsync(DirectoryInfo directory, CancellationToken token);
        Task<DirectoryContent> GetDirectoryContentAsync(DirectoryInfo directory, CancellationToken token);
    }
}
