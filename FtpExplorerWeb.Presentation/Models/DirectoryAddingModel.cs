using System.ComponentModel.DataAnnotations;

namespace FtpExplorerWeb.Presentation.Models
{
    public class DirectoryAddingModel
    {
        [Required(ErrorMessage = "Enter folder name")]
        public string DirectoryName { get; set; } = "";
    }
}
