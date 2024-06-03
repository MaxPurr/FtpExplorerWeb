using FtpExplorerWeb.DataAccess.Clients;
using FtpExplorerWeb.Domain.Entities;
using FtpExplorerWeb.Domain.Interfaces;
using FtpExplorerWeb.Domain.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FtpExplorerWeb.Utils.Verifiers
{
    public class FtpConnectionVerifier : IFtpConnectionVerifier
    {
        public async Task<bool> VerifyConnectionAsync(FtpConnectionOptions options, CancellationToken token)
        {
            var ftpClient = new FtpClient(options);
            try
            {
                await ftpClient.GetDirectoryContentAsync(FileInfoBase.RootDirectory, token);
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}
