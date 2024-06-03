namespace FtpExplorerWeb.Domain.Entities
{
    public class FileInfo : FileInfoBase
    {
        private readonly string _name;
        private readonly string? _extension;
        private const char _extensionSeparator = '.';

        public FileInfo(string name, string? extension = null, DirectoryInfo? directory = null) :
            base(directory)
        {
            _name = name;
            _extension = extension;
        }
        public override string Name
        {
            get
            {
                if (_extension == null)
                {
                    return _name;
                }
                return $"{_name}{_extensionSeparator}{_extension}";
            }
        }
    }
}
