using Core.Enums;
using System.IO.Abstractions;

namespace Services.Parsers
{
    public class InstagramJsonParser : MetaJsonParser
    {
        public InstagramJsonParser(IFileSystem fileSystem) : base(fileSystem) { }

        public override string Source => "Instagram";
        public override MessageParsers ParserType => MessageParsers.InstagramHtml;
    }
}
