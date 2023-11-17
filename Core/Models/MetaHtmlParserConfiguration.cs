namespace Core.Models
{
    public class MetaHtmlParserConfiguration : MessageParserConfiguration
    {
        public bool WasAutoCalculated { get; set; } = false;
        public string MessageHeaderIdentifer { get; set; } = string.Empty;
        public string SenderNodeIdentifier { get; set; } = string.Empty;
        public string TimestampNodeIdentifier { get; set; } = string.Empty;
        public string ContentNodeIdentifier { get; set; } = string.Empty;
    }
}
