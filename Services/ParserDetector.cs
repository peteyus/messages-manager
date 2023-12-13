namespace Services
{
    using Core.Extensions;
    using Core.Interfaces;
    using Core.Models;
    using System.IO.Abstractions;

    public class ParserDetector : IParserDetector
    {
        private readonly IFileSystem fileSystem;

        public ParserDetector(IFileSystem fileSystem)
        {
            fileSystem.ThrowIfNull(nameof(fileSystem));

            this.fileSystem = fileSystem;
        }

        public MessageParserConfiguration DetectParser(string filePath)
        {
            // for now, keep it simple
            if (!this.fileSystem.File.Exists(filePath))
            {
                throw new FileNotFoundException(Core.Strings.ErrorFileNotFound.FormatCurrentCulture(filePath));
            }
            
            var config = new MessageParserConfiguration();
            switch (this.fileSystem.Path.GetExtension(filePath))
            {
                case "html":
                    config.Parser = Core.Enums.MessageParsers.InstagramHtml;
                    break;
                case "json":
                case "txt":
                case "":
                    config.Parser = Core.Enums.MessageParsers.InstagramJson;
                    break;
                default:
                    throw new InvalidCastException(Core.Strings.ErrorUnknownFileFormat.FormatCurrentCulture(filePath));
            }

            return config;
        }
    }
}
