namespace Core.Interfaces
{
    using Core.Models;

    public interface IMessageParser
    {
        public IEnumerable<Message> ReadMessagesFromFile(string sourceFilePath, MessageParserConfiguration? options = null);

        public IEnumerable<Message> ReadMessages(string messageContent, MessageParserConfiguration? options = null);

        public MessageSample ConfigureParsingAndReturnSample(string sourceFilePath, MessageParserConfiguration? options = null);
    }
}
