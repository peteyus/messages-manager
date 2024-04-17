using Core.Enums;
using Core.Interfaces;
using Core.Models;
using System.IO.Compression;

namespace Services.Parsers
{
    public class ZipFileParser : IMessageParser
    {
        public MessageParsers ParserType => MessageParsers.ZipFile;

        public MessageSample ConfigureParsingAndReturnSample(string sourceFilePath, MessageParserConfiguration? options = null)
        {
            // You're going to need to return the configuration inside the sample and indicate that we need to know which file(s)
            // contain the messages to import. At this point, they'll be uncompressed, so we'll also need another round of
            // parser detection on the decompressed files after the "master" file is selected.
            if (options is not ZipFileParserConfiguration zipConfiguration)
            {
                // TODO PRJ: Resource-ify these strings.
                throw new InvalidCastException("The configuration for zip files must be a zip config.");
            }

             return new MessageSample { ParserConfiguration = zipConfiguration, ParseSuccessful = true };
        }

        public IEnumerable<Message> ReadMessages(string messageContent, MessageParserConfiguration? options = null)
        {
            throw new NotImplementedException("Reading messages directly from a zip file is not supported.");
        }

        public IEnumerable<Message> ReadMessagesFromFile(string sourceFilePath, MessageParserConfiguration? options = null)
        {
            throw new NotImplementedException("Reading messages directly from a zip file is not supported.");
        }
    }
}
