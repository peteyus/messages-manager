using System.CommandLine.Builder;

namespace Messages.CLI.Interfaces
{
    public interface ICommandLineBuilderInvoker
    {
        Task<int> InvokeAsync(CommandLineBuilder commandLineBuilder, string[] args);
    }
}
