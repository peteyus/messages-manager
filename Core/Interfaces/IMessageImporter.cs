using Core.Models;

namespace Core.Interfaces
{
    public interface IMessageImporter
    {
        /// <summary>
        /// This method will attempt to import a file and parse out a message, 
        /// and will use detection to identify the file format and guess which parser to use
        /// </summary>
        /// <param name="filePath">The path to the file that is being imported</param>
        /// <param name="configuration">Optional configuration to specify which type of parser to use</param>
        /// <returns>A <see cref="MessageSample"/> with the configuration used and a sample of the message parsed out</returns>
        MessageSample PreviewFileImport(string filePath, MessageParserConfiguration? configuration);

        /// <summary>
        /// This will create a <see cref="Conversation"/> from the file at the given path with the parser configuration. 
        /// </summary>
        /// <param name="filePath">The path to the file that is being imported</param>
        /// <param name="configuration">Optional configuration to specify which parser to use. If not provided, best guess will be applied.</param>
        /// <param name="conversationId">If provided, it will link the messages to an existing conversation.</param>
        /// <returns>A <see cref="Conversation"/> on success with the Id populated for detailed querying</returns>
        Conversation ImportConversationFromFile(string filePath, MessageParserConfiguration? configuration, int? conversationId);
    }
}
