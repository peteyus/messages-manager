namespace Services
{
    using Core.Extensions;
    using Core.Interfaces;
    using Core.Models;
    using System.IO.Abstractions;
    using System.IO.Compression;

    public class ParserDetector : IParserDetector
    {
        private readonly IFileSystem fileSystem;
        private readonly IUnzipService unzipService;

        public ParserDetector(IFileSystem fileSystem, IUnzipService unzipService)
        {
            fileSystem.ThrowIfNull(nameof(fileSystem));
            unzipService.ThrowIfNull(nameof(unzipService));

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
            }
            return config;
        }
        
    }
}
