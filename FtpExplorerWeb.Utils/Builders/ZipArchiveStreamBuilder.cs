using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;

namespace FtpExplorerWeb.Utils.Builders
{
    public class ZipArchiveStreamBuilder
    {
        private static string _zipExtension = ".zip";
        private readonly MemoryStream _stream;
        private readonly ZipArchive _archive;
        public ZipArchiveStreamBuilder()
        {
            _stream = new MemoryStream();
            _archive = new ZipArchive(_stream, ZipArchiveMode.Create, true);
        }

        public async Task AddFileAsync(string fileName, Stream sourseStream, CancellationToken token)
        {
            var entry = _archive.CreateEntry(fileName, CompressionLevel.NoCompression);
            using (var entryStream = entry.Open())
            {
                await sourseStream.CopyToAsync(entryStream);
            }
        }

        public async Task AddZipArchiveAsync(string archiveName, Stream sourseStream, CancellationToken token)
        {
            await AddFileAsync(archiveName + _zipExtension, sourseStream, CancellationToken.None);
        }

        public MemoryStream ToStream()
        {
            _archive.Dispose();
            _stream.Position = 0;
            return _stream;
        }
    }
}
