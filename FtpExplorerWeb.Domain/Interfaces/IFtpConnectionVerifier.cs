using FtpExplorerWeb.Domain.Options;
using System.Threading;
using System.Threading.Tasks;

namespace FtpExplorerWeb.Domain.Interfaces
{
    public interface IFtpConnectionVerifier
    {
        Task<bool> VerifyConnectionAsync(FtpConnectionOptions options, CancellationToken token);
    }
}
