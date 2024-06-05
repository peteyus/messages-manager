using System.CommandLine.Builder;

namespace Messages.CLI.Interfaces
{
    public interface ICommandLineBuilderFactory
    {
        CommandLineBuilder GetBuilder();
    }
}
