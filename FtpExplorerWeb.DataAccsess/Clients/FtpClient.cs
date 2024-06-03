using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using FtpExplorerWeb.Domain.Entities;
using FtpExplorerWeb.Domain.Extensions;
using FtpExplorerWeb.Domain.Helpers;
using FtpExplorerWeb.Domain.Interfaces;
using FtpExplorerWeb.Domain.Options;

using DirectoryInfo = FtpExplorerWeb.Domain.Entities.DirectoryInfo;
using FileInfo = FtpExplorerWeb.Domain.Entities.FileInfo;

namespace FtpExplorerWeb.DataAccess.Clients
{
    public class FtpClient : IFtpClient
    {
        private const string _ftpPrefix = "ftp://";
        private string _uri;
        private NetworkCredential _networkCredential;
        public FtpClient(FtpConnectionOptions options) { 
            _uri = _ftpPrefix + options.Uri;
            _networkCredential = new NetworkCredential(options.User, options.Password);
        }
        private FtpWebRequest CreateFtpWebRequest(string method, string directory = "")
        {
            string uri = Path.Combine(_uri, directory);
            var request = (FtpWebRequest)WebRequest.Create(uri);
            request.Method = method;
            request.Credentials = _networkCredential;
            request.UsePassive = true;
            request.UseBinary = true;
            request.EnableSsl = true;
            return request;
        }
        public async Task<FileInfo> UploadFileAsync(DirectoryInfo destinationDirectory, string sourseFileName, Stream sourseStream, CancellationToken token)
        {
            string filePath = Path.Combine(destinationDirectory.GetPath(), sourseFileName);
            var request = CreateFtpWebRequest(WebRequestMethods.Ftp.UploadFile, filePath);
            await using (Stream requestStream = await request.GetRequestStreamAsync())
            {
                await sourseStream.CopyToAsync(requestStream, token);
            }
            return FileInfoHelper.GetFileInfoFromFullName(sourseFileName, destinationDirectory);
        }
        public async Task<Stream> GetFileDownloadStreamAsync(FileInfo sourseFile, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            string sourseFilePath = sourseFile.GetPath();
            var request = CreateFtpWebRequest(WebRequestMethods.Ftp.DownloadFile, sourseFilePath);
            var response = (FtpWebResponse)(await request.GetResponseAsync());
            return response.GetResponseStream();
        }
        public async Task<DirectoryInfo> MakeDirectoryAsync(DirectoryInfo destinationDirectory, string soursedirectoryName, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            string directoryPath = Path.Combine(destinationDirectory.GetPath(), soursedirectoryName);
            var request = CreateFtpWebRequest(WebRequestMethods.Ftp.MakeDirectory, directoryPath);
            var response = (FtpWebResponse)(await request.GetResponseAsync());

            Console.WriteLine(response.StatusCode);
            Console.WriteLine(response.StatusDescription);
            foreach (var header in response.Headers)
            {
                Console.WriteLine(header);
            }
            return new DirectoryInfo(soursedirectoryName, destinationDirectory);
        }
        public async Task DeleteFileAsync(FileInfo file, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            string filePath = file.GetPath();
            var request = CreateFtpWebRequest(WebRequestMethods.Ftp.DeleteFile, filePath);
            await request.GetResponseAsync();
        }
        public async Task DeleteFilesAsync(IEnumerable<FileInfo> files, CancellationToken token)
        {
            ParallelOptions parallelOptions = new ParallelOptions()
            {
                CancellationToken = token,
                MaxDegreeOfParallelism = files.Count(),
            };
            await Parallel.ForEachAsync(files, parallelOptions, async (file, cancellationToken) =>
            {
                await DeleteFileAsync(file, cancellationToken);
            });
        }
        public async Task RemoveDirectoryAsync(DirectoryInfo directory, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            string directoryPath = directory.GetPath();
            await ClearDirectoryAsync(directory, token);
            var request = CreateFtpWebRequest(WebRequestMethods.Ftp.RemoveDirectory, directoryPath);
            await request.GetResponseAsync();
        }
        public async Task RemoveDirectoriesAsync(IEnumerable<DirectoryInfo> directories, CancellationToken token)
        {
            ParallelOptions parallelOptions = new ParallelOptions()
            {
                CancellationToken = token,
                MaxDegreeOfParallelism = directories.Count(),
            };
            await Parallel.ForEachAsync(directories, parallelOptions, async (directory, cancellationToken) =>
            {
                await RemoveDirectoryAsync(directory, cancellationToken);
            });
        }
        public async Task RemoveFilesAndDirectoriesAsync(IEnumerable<FileInfo> files, IEnumerable<DirectoryInfo> directories, CancellationToken token)
        {
            Task deleteFilesTask = Task.CompletedTask;
            Task removeDirectoriesTask = Task.CompletedTask;
            if (files.Count() > 0)
            {
                deleteFilesTask = DeleteFilesAsync(files, token);
            }
            if(directories.Count() > 0)
            {
                removeDirectoriesTask = RemoveDirectoriesAsync(directories, token);
            }
            await Task.WhenAll(deleteFilesTask, removeDirectoriesTask);
        }
        public async Task ClearDirectoryAsync(DirectoryInfo directory, CancellationToken token)
        {
            var directoryContent = await GetDirectoryContentAsync(directory, token);
            await RemoveFilesAndDirectoriesAsync(directoryContent.Files, directoryContent.Directories, token);
        }
        public async Task<DirectoryContent> GetDirectoryContentAsync(DirectoryInfo directory, CancellationToken token)
        {
            var directoryContent = new DirectoryContent(directory);
            var request = CreateFtpWebRequest(WebRequestMethods.Ftp.ListDirectoryDetails, directory.GetPath());
            FtpWebResponse response = (FtpWebResponse)await request.GetResponseAsync();
            using (Stream responseStream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(responseStream))
            {
                for (int i = 0; i < 2; ++i)
                {
                    await reader.ReadLineAsync(token);
                }
                string? fileData;
                while ((fileData = await reader.ReadLineAsync(token)) != null)
                {
                    directoryContent.FillFromFileData(fileData);
                }
            }
            return directoryContent;
        }
    }
}
