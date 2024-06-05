namespace Messages.CLI
{
    using Microsoft.Extensions.DependencyInjection;
    using System.CommandLine.Builder;
    using System.CommandLine;
    using Messages.CLI.Commands;
    using Core.Extensions;
    using Messages.CLI.Interfaces;

    public class CommandLineBuilderFactory : ICommandLineBuilderFactory
    {
        private readonly IServiceProvider serviceProvider;

        public CommandLineBuilderFactory(IServiceProvider serviceProvider)
        {
            serviceProvider.ThrowIfNull(nameof(serviceProvider));
            this.serviceProvider = serviceProvider;
        }

        public CommandLineBuilder GetBuilder()
        {
            var rootCommand = new RootCommand("Messages Manager command-line interface.");

            var commandsToAdd = this.serviceProvider.GetServices<BaseCommand>().ToList();

            // Sort commands alphabetically for help text
            foreach (var command in commandsToAdd.OrderBy(c => c.Name))
            {
                rootCommand.Add(command);
            }

            return new CommandLineBuilder(rootCommand);
        }
    }
}
