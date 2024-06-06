using Core.Interfaces;
using System.CommandLine;
using System.CommandLine.Invocation;

namespace Messages.CLI.Commands.Index.AddFile
{
    public class IndexAddFileCommand : CliCommand
    {
        // TODO PRJ: Not functional. Or even right yet.
        private static readonly Option<string> filePathOption = new Option<string>("--filePath", "Path to the file to read in.");

        private readonly IEnumerable<IMessageParser> _parsers;

        public IndexAddFileCommand(
            IEnumerable<IMessageParser> parsers
            )
            : base("addFile", "Reads in a file, parses it for messages, and if found indexes them in the configured index.")
        {
            _parsers = parsers;

            AddOption(filePathOption);
            this.SetHandler(
                async (filePath) =>
                {
                    await ExecuteWithExitCodeAsync(ExecuteAsync(filePath));
                }, filePathOption);
        }

        public Task ExecuteAsync(string filePath)
        {
            return Task.CompletedTask;
        }
    }
}
