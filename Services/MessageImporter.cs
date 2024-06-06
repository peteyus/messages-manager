namespace Services
{
    using Core.Extensions;
    using Core.Interfaces;
    using Core.Models;
    using System;
    using System.IO.Abstractions;

    public class MessageImporter : IMessageImporter
    {
        private readonly IFileSystem fileSystem;
        private readonly IParserDetector parserDetector;
        private readonly IMessageRepository repository;

        public MessageImporter(
            IFileSystem fileSystem, 
            IParserDetector parserDetector, 
            IMessageRepository repository)
        {
            fileSystem.ThrowIfNull(nameof(fileSystem));
            parserDetector.ThrowIfNull(nameof(parserDetector));
            repository.ThrowIfNull(nameof(repository));

            this.fileSystem = fileSystem;
            this.parserDetector = parserDetector;
            this.repository = repository;
        }

        public Conversation ImportConversationFromFile(string filePath, MessageParserConfiguration? configuration, int? conversationId)
        {
            filePath.ThrowIfNullOrEmpty(nameof(filePath));

            Conversation conversation;
            if (conversationId != null)
            {
                conversation = this.repository.GetConversation(conversationId.Value);
            } else
            {
                conversation = new Conversation();

            }

            if (configuration == null)
            {
                configuration = this.DetectConfigForFile(filePath);
            }

            var parser = this.parserDetector.GetParser(configuration);
            var messages = parser.ReadMessagesFromFile(filePath, configuration);

            this.repository.ImportMessagesToConversation(conversation, messages);
            this.repository.SaveConversation(conversation);
            return conversation;
        }

        // Front end will call for preview, then will call import once preview is successful.
        public MessageSample PreviewFileImport(string filePath, MessageParserConfiguration? configuration)
        {
            filePath.ThrowIfNullOrEmpty(nameof(filePath));

            MessageParserConfiguration config = this.DetectConfigForFile(filePath);
            config.ThrowIfNull(nameof(config));

            var parser = this.parserDetector.GetParser(config);

            return parser.ConfigureParsingAndReturnSample(filePath, config);
        }

        private MessageParserConfiguration DetectConfigForFile(string filePath)
        {
            try
            {
                return this.parserDetector.DetectParser(filePath);
            }
            catch (InvalidCastException)
            {
                // TODO PRJ: Better way to default/detect default? Better error handling in ParserDetector?
                return new MessageParserConfiguration { Parser = Core.Enums.MessageParsers.InstagramJson };
            }
        }
    }
}
