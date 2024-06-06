namespace Messages.CLI.Commands
{
    public abstract class BaseCommand : CliCommand
    {
        /// <summary>
        /// Basecommands are direct children of the RootCommand and should be the first term 
        /// after the executable.
        /// </summary>
        /// <param name="name">The name of the command.</param>
        /// <param name="description">Description of the command. This is part of the help text.</param>
        protected BaseCommand(string name, string description) : base(name, description)
        {
        }
    }
}
