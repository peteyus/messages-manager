using Core.Enums;
using Core.Models.Application;

namespace Core.Models
{
    public class ZipFileParserConfiguration : MessageParserConfiguration
    {
        public ZipFileParserConfiguration(Folder folder)
        {
            this.Folder = folder;
        }

        public override MessageParsers Parser => MessageParsers.ZipFile;

        public Folder Folder { get; set; }
    }
}
