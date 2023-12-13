using Core.Enums;

namespace Services.Parsers
{
    public class InstagramHtmlParser : MetaHtmlParser
    {
        public override string SourceName => "Instagram";
        public override MessageParsers ParserType => MessageParsers.InstagramHtml;
    }
}
