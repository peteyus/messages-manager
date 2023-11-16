namespace Core.Models
{
    public class MetaHtmlParserConfiguration : MessageParserConfiguration
    {
        public string MessageHeaderIdentifer { get; set; } = "_2lej";
        public string SenderNodeIdentifier { get; set; } = "_2lek";
        public string TimestampNodeIdentifier { get; set; } = "_2lem";
        public string ContentNodeIdentifier { get; set; } = "_2let";
    }
}
