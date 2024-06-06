using Messages.CLI.Commands.Index.AddFile;
using Messages.CLI.Commands.Index.Create;
using Messages.CLI.Commands.Index.Search;

namespace Messages.CLI.Commands.Index
{
    public class IndexCommand : BaseCommand
    {
        const string name = "index";
        const string description = "Interacts with the search index.";

        public IndexCommand(
            IndexAddFileCommand addfileCommand,
            IndexCreateCommand createCommand,
            IndexSearchCommand searchCommand
            ) : base(name, description)
        {
            this.AddCommand(addfileCommand);
            this.AddCommand(createCommand);
            this.AddCommand(searchCommand);
        }
    }
}
