using System.Reflection.Metadata;

namespace Messages.CLI.Commands.Index.Search
{
    public class IndexSearchCommand : CliCommand
    {
        // TODO PRJ: Not functional yet.
        const string name = "search";
        const string description = "Searches the configured index.";
        public IndexSearchCommand() : base(name, description)
        {
        }
    }
}
