using System.IO.Abstractions;

namespace Services.Parsers
{
    public class FacebookJsonParser : MetaJsonParser
    {
        public FacebookJsonParser(IFileSystem fileSystem) : base(fileSystem) { }

        public override string Source => "Facebook";
    }
}
