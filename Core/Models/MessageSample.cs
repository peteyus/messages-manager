using Core.Enums;

namespace Core.Models
{
    public class MessageSample
    {
        public Message? SampleMessage { get; set; }
        public bool ParseSuccessful { get; set; } = false;
        public MessageParsers? Parser => ParserConfiguration?.Parser;
        public MessageParserConfiguration? ParserConfiguration { get; set; }
    }
}
