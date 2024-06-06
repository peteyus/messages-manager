namespace Services
{
    using Core;
    using Core.Enums;
    using Core.Extensions;
    using Core.Interfaces;
    using Core.Models;
    using System.IO.Abstractions;

    public class ParserDetector : IParserDetector
    {
        private readonly IEnumerable<IMessageParser> parsers;
        private readonly IFileSystem fileSystem;
        private readonly IUnzipService unzipService;

        public ParserDetector(
            IEnumerable<IMessageParser> parsers, 
            IFileSystem fileSystem, 
            IUnzipService unzipService)
        {
            this.parsers = parsers;
            this.fileSystem = fileSystem;
            this.unzipService = unzipService;
        }

        public MessageParserConfiguration DetectParser(string filePath)
        {
            // for now, keep it simple
            if (!this.fileSystem.File.Exists(filePath))
            {
                throw new FileNotFoundException(Core.Strings.ErrorFileNotFound.FormatCurrentCulture(filePath));
            }

            var config = new MessageParserConfiguration();
            var extension = this.fileSystem.Path.GetExtension(filePath);
            if (extension == ".zip" || extension == ".gz" || extension == ".7z")
            {
                // need to handle zip files different - first we unzip, then we parse
                // gzip and 7zip are probably a little extra. Focus on zip for now
                // 
                var directoryStructure = this.unzipService.UnzipFile(filePath);
                config = new ZipFileParserConfiguration(directoryStructure);
            }
            else
            {
                switch (extension)
                {
                    case "html":
                        config.Parser = MessageParsers.InstagramHtml;
                        break;
                    case "json":
                    case "txt":
                    case "":
                        config.Parser = MessageParsers.InstagramJson;
                        break;
                    default:
                        throw new InvalidCastException(Strings.ErrorUnknownFileFormat.FormatCurrentCulture(filePath));
                }
            }
            return config;
        }

        public IMessageParser GetParser(MessageParserConfiguration config)
        {
            var parser = this.parsers.FirstOrDefault(p => p.ParserType == config.Parser);
            if (parser == null)
            {
                throw new Exception(Strings.ErrorParserNotFound.FormatCurrentCulture(Enum.GetName(config.Parser) ?? config.Parser.ToString()));
            }

            return parser;
        }

        public IMessageParser GetParser(MessageParsers parserType)
        {
            var parser = this.parsers.FirstOrDefault(p => p.ParserType == parserType);
            if (parser == null)
            {
                throw new Exception(Strings.ErrorParserNotFound.FormatCurrentCulture(Enum.GetName(parserType) ?? parserType.ToString()));
            }

            return parser;
        }
    }
}
