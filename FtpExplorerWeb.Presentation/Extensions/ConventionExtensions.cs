using FtpExplorerWeb.Domain.Options;
using FtpExplorerWeb.Presentation.Models;

namespace FtpExplorerWeb.Presentation.Extensions
{
    public static class ConventionExtensions
    {
        public static FtpConnectionOptions ToFtpConnectionOptions(this FtpConnectionModel model)
        {
            return new FtpConnectionOptions()
            {
                Uri = model.Uri,
                User = model.User,
                Password = model.Password,
            };
        }
    }
}
