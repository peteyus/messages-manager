using Messages.CLI.Interfaces;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Help;
using System.CommandLine.Invocation;
using System.CommandLine.IO;
using System.CommandLine.Parsing;

namespace Messages.CLI
{
    public class CommandLineBuilderInvoker : ICommandLineBuilderInvoker
    {
        public Task<int> InvokeAsync(CommandLineBuilder commandLineBuilder,
            string[] args)
        {
            return commandLineBuilder
                .UseDefaults()
                .UseHelpBuilder(context =>
                {
                    if (context.ParseResult.Tokens.Count == 0
                        || context.ParseResult.CommandResult.Command.GetType() == typeof(RootCommand))
                    {
                        return new DefaultHelpBuilder();
                    }

                    return new HelpBuilder(LocalizationResources.Instance);
                })
                .UseExceptionHandler((exception, context) => context.Console.Out.WriteLine(
                    string.Format("An unknown error has occurred: {0}",
                        exception.Message)))
                .Build()
                .InvokeAsync(args);
        }
    }
}
