namespace Core.Interfaces
{
    using Core.Models;

    public interface IMessageParser
    {
        public IEnumerable<Message> ReadMessagesFromFile(string sourceFilePath, MessageParserConfiguration? options = null); // TODO PRJ: Source? Always assume file? Config object?

        public IEnumerable<Message> ReadMessages(string messageContent, MessageParserConfiguration? options = null);
    }
}
