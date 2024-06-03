using System.ComponentModel.DataAnnotations;

namespace FtpExplorerWeb.Presentation.Models
{
    public class FtpConnectionModel
    {
        public string Uri { get; set; } = "";
        public string User { get; set; } = "";
        public string Password { get; set; } = "";
    }
}
