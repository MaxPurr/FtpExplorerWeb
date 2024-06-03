namespace FtpExplorerWeb.Domain.Options
{
    public class FtpConnectionOptions
    {
        public required string Uri { get; init; }
        public required string User { get; init; }
        public required string Password { get; init; }
    }
}
