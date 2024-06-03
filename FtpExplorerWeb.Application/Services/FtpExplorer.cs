using System.Collections;
using System.Collections.ObjectModel;
using FtpExplorerWeb.Domain.Interfaces;
using FtpExplorerWeb.Domain.Entities;
using FtpExplorerWeb.Utils.Builders;
using DirectoryInfo = FtpExplorerWeb.Domain.Entities.DirectoryInfo;
using FileInfo = FtpExplorerWeb.Domain.Entities.FileInfo;

namespace FtpExplorerWeb.Application.Services
{
public class FtpExplorer
{
    private static string _defaultZipArchiveName = "archive.zip";
    private IFtpClientProvider _ftpClientProvider;
    private DirectoryInfo _currentDirectory;
    private List<FileInfo> _files;
    private List<DirectoryInfo> _directories;
    private List<FileInfo> _selectedFiles;
    private List<DirectoryInfo> _selectedDirectories;
    private bool _isInitialized;
    public FtpExplorer(IFtpClientProvider ftpClientProvider)
    {
        _ftpClientProvider = ftpClientProvider;
        _currentDirectory = FileInfoBase.RootDirectory;
        _files = new();
        _directories = new();
        _selectedFiles = new();
        _selectedDirectories = new();
        _isInitialized = false;
    }
    public DirectoryInfo CurrentDirectory => _currentDirectory;
    public ReadOnlyCollection<FileInfo> Files => _files.AsReadOnly();
    public ReadOnlyCollection<DirectoryInfo> Directories => _directories.AsReadOnly();
    public int TotalFiles => _files.Count + _directories.Count;
    public int TotalSelected => _selectedFiles.Count + _selectedDirectories.Count;
    public bool IsInitialized => _isInitialized;
    private IFtpClient FtpClient => _ftpClientProvider.FtpClient;
    public async Task InitializeAsync(CancellationToken token)
    {
        if (_isInitialized)
        {
            return;
        }
        await ReloadFilesAsync(token);
        _isInitialized = true;
    }
    public void ClearSelectedFiles()
    {
        _selectedFiles.Clear();
        _selectedDirectories.Clear();
    }
    public bool IsFileSelected(FileInfoBase baseFile)
    {
        if (baseFile is FileInfo)
        {
            return _selectedFiles.Contains(baseFile);
        }
        else if (baseFile is DirectoryInfo) 
        {
            return _selectedDirectories.Contains(baseFile);
        }
        return false;
    }
    private IList GetDestinationListForSelectedFile(FileInfoBase baseFile)
    {
        IList destinationList;
        if (baseFile is FileInfo file)
        {
            if (!_files.Contains(baseFile))
            {
                throw new ArgumentException("File not found in current directory.");
            }
            destinationList = _selectedFiles;
        }
        else
        {
            if (!_directories.Contains(baseFile))
            {
                throw new ArgumentException("Directory not found in current directory.");
            }
            destinationList = _selectedDirectories;
        }
        return destinationList;
    }
    public void SelectOrDeselectFile(FileInfoBase baseFile)
    {
        var destinationList = GetDestinationListForSelectedFile(baseFile);
        bool wasSelected = destinationList.Contains(baseFile);
        ClearSelectedFiles();
        if (!wasSelected)
        {
            destinationList.Add(baseFile);
        }
    }
    public void SelectOrDeselectFiles(FileInfoBase baseFile)
    {
        var destinationList = GetDestinationListForSelectedFile(baseFile);
        if (destinationList.Contains(baseFile))
        {
            destinationList.Remove(baseFile);
            return;
        }
        destinationList.Add(baseFile);
    }
    public async Task ReloadFilesAsync(CancellationToken token)
    {
        _files.Clear();
        _directories.Clear();
        ClearSelectedFiles();
        var content = await FtpClient.GetDirectoryContentAsync(_currentDirectory, token);
        _files.AddRange(content.Files);
        _directories.AddRange(content.Directories);
    }
    public async Task UploadFileAsync(string fileName, Stream fileStream, CancellationToken token)
    {
        var file = await FtpClient.UploadFileAsync(_currentDirectory, fileName, fileStream, token);
        _files.Add(file);
    }
    public async Task<(string, Stream)> GetDownloadZipArchiveStreamAsync(CancellationToken token)
    {
        Stream stream;
        if (_selectedFiles.Count == 1 && _selectedDirectories.Count == 0)
        {
            var file = _selectedFiles[0];
            stream = await FtpClient.GetFileDownloadStreamAsync(file, token);
            return (file.Name, stream);
        }
        if (_selectedFiles.Count == 0 && _selectedDirectories.Count == 1)
        {
            var directory = _selectedDirectories[0];
            stream = await GetDirectoryDownloadStreamAsync(directory, token);
            return (directory.Name + ".zip", stream);
        }
        stream = await CreateZipArchiveStreamAsync(_selectedFiles, _selectedDirectories, token);
        return (_defaultZipArchiveName, stream);
    }
    private async Task<MemoryStream> CreateZipArchiveStreamAsync(IEnumerable<FileInfo> files, IEnumerable<DirectoryInfo> directories, CancellationToken token)
    {
        var archiveStreamBuilder = new ZipArchiveStreamBuilder();
        foreach (var file in files)
        {
            await using(var stream = await FtpClient.GetFileDownloadStreamAsync(file, token))
            {
                await archiveStreamBuilder.AddFileAsync(file.Name, stream, token);
            }
        }
        foreach (var directory in directories)
        {
            await using (var stream = await GetDirectoryDownloadStreamAsync(directory, token))
            {
                await archiveStreamBuilder.AddZipArchiveAsync(directory.Name, stream, token);
            }
        }
        return archiveStreamBuilder.ToStream();
    }
    private async Task<MemoryStream> GetDirectoryDownloadStreamAsync(DirectoryInfo directory, CancellationToken token)
    {
        var content = await FtpClient.GetDirectoryContentAsync(directory, token);
        var stream = await CreateZipArchiveStreamAsync(content.Files, content.Directories, token);
        return stream;
    }
    public async Task DeleteSelectedFilesAsync(CancellationToken token)
    {
        await FtpClient.RemoveFilesAndDirectoriesAsync(_selectedFiles, _selectedDirectories, token);
        foreach (var file in _selectedFiles)
        {
            _files.Remove(file);
        }
        foreach (var directory in _selectedDirectories)
        {
            _directories.Remove(directory);
        }
        ClearSelectedFiles();
    }
    public async Task MoveToParentDirectoryAsync(CancellationToken token)
    {
        if(_currentDirectory == FileInfoBase.RootDirectory)
        {
            return;
        }
        await MoveToDirectoryAsync(_currentDirectory.Directory, token);
    }
    public async Task MoveToRootDirectoryAsync(CancellationToken token)
    {
        await MoveToDirectoryAsync(FileInfoBase.RootDirectory, token);
    }
    public async Task MoveToDirectoryAsync(DirectoryInfo directory, CancellationToken token)
    {
        _currentDirectory = directory;
        await ReloadFilesAsync(token);
    }
    public async Task MakeDirectoryAsync(string directoryName, CancellationToken token)
    {
        await FtpClient.MakeDirectoryAsync(_currentDirectory, directoryName, token);
        var directoryInfo = new DirectoryInfo(directoryName, _currentDirectory);
        _directories.Add(directoryInfo);
    }
}
}
