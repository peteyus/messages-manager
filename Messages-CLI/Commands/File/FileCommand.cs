using Messages.CLI.Commands.File.Parse;

namespace Messages.CLI.Commands.File
{
    public class FileCommand : BaseCommand
    {
        const string name = "file";
        const string description = "Provides commands for manipulating and parsing files.";

        public FileCommand(
            FileParseCommand parseCommand
            ) : base(name, description)
        {
            this.AddCommand(parseCommand);
        }
    }
}
