using Core.Enums;
using Core.Interfaces;
using Messages.CLI.Options;
using System.CommandLine;
using System.IO.Abstractions;
using FileOptions = Messages.CLI.Options.FileOptions;

namespace Messages.CLI.Commands.File.Parse
{
    public class FileParseCommand : CliCommand
    {
        const string name = "parse";
        const string description = "Parses a provided filename into a collection of messages.";

        private readonly IFileSystem _fileSystem;
        private readonly IParserDetector _parserDetector;

        public FileParseCommand(
            IFileSystem fileSystem,
            IParserDetector parserDetector
            ) : base(name, description)
        {
            _fileSystem = fileSystem;
            _parserDetector = parserDetector;

            var fileOption = FileOptions.BuildFileOption(_fileSystem);
            var parserOption = ParserOptions.ParserOption;

            this.AddOption(fileOption);
            this.AddOption(parserOption);

            this.SetHandler(this.Execute, 
                fileOption, 
                parserOption);
        }

        private Task Execute(IFileInfo? file, MessageParsers parserType)
        {
            if (file is null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            if (parserType == MessageParsers.Unknown)
            {
                var parserConfiguration = _parserDetector.DetectParser(file.FullName);
                parserType = parserConfiguration.Parser;
            }

            var parser = _parserDetector.GetParser(parserType);
            var conversation = parser.ReadMessagesFromFile(file.FullName);

            return Task.CompletedTask;
        }
    }
}
