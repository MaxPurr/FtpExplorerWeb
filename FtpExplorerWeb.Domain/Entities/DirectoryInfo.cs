namespace FtpExplorerWeb.Domain.Entities
{
    public class DirectoryInfo : FileInfoBase
    {
        private readonly string _name;
        public DirectoryInfo(string name, DirectoryInfo? directory = null) 
            : base(directory)
        {
            _name = name;
        }
        public override string Name => _name;
    }
}
