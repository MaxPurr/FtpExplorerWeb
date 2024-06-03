using FtpExplorerWeb.Domain.Options;
using System.Threading.Tasks;
using System.Threading;

namespace FtpExplorerWeb.Domain.Interfaces
{
    public interface IFtpClientProvider
    {
        IFtpClient FtpClient { get; }
        Task RegisterFtpClientAsync(FtpConnectionOptions options);
    }
}
