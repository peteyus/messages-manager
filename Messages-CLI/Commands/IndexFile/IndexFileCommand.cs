using Core.Interfaces;
using System.CommandLine;
using System.CommandLine.Invocation;

namespace Messages.CLI.Commands.IndexFile
{
    public class IndexFileCommand : BaseCommand
    {
        private static readonly Option<string> filePathOption = new Option<string>("--filePath", "Path to the file to read in.");

        private readonly IEnumerable<IMessageParser> _parsers;

        public IndexFileCommand(
            IEnumerable<IMessageParser> parsers
            )
            : base("indexfile", "Reads in a file, parses it for messages, and if found indexes them in the configured index.")
        {
            _parsers = parsers;

            AddOption(filePathOption);
            this.SetHandler(
                async (string filePath) => {
                    await ExecuteWithExitCodeAsync(this.ExecuteAsync(filePath)); 
                }, filePathOption);
        }

        public Task ExecuteAsync(string filePath)
        {
            return Task.CompletedTask;
        }
    }
}
