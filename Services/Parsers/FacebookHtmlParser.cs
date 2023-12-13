namespace Services.Parsers
{
    using Core.Enums;

    public class FacebookHtmlParser : MetaHtmlParser
    {
        public override string SourceName => "Facebook";
        public override MessageParsers ParserType => MessageParsers.FacebookHtml;
    }
}
