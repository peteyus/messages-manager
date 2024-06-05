using Messages.CLI.Interfaces;

namespace Messages.CLI
{
    public class CliExecutor : ICliExecutor
    {
        private readonly ICommandLineBuilderFactory commandLineBuilderFactory;
        private readonly ICommandLineBuilderInvoker commandLineBuilderInvoker;

        public CliExecutor(
            ICommandLineBuilderFactory commandLineBuilderFactory,
            ICommandLineBuilderInvoker commandLineBuilderInvoker)
        {
            this.commandLineBuilderFactory = commandLineBuilderFactory;
            this.commandLineBuilderInvoker = commandLineBuilderInvoker;
        }

        public Task<int> ExecuteAsync(string[] args)
        {
            var commandLine = this.commandLineBuilderFactory.GetBuilder();
            return this.commandLineBuilderInvoker.InvokeAsync(commandLine, args);
        }
    }
}
