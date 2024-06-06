using Core.Extensions;
using Microsoft.Extensions.Logging;
using System.CommandLine;
using System.Resources;
using System.Runtime.InteropServices;

namespace Messages.CLI.Commands
{
    /// <summary>
    /// CliCommands are subcommands of Basecommands
    /// </summary>
    public abstract class CliCommand : Command
    {
        private readonly ILogger? _logger;

        protected CliCommand(string name, string description)
            : base(name, description)
        {
        }

        /// <summary>
        /// Creates a new command instance with an ILogger. The logger is used to report success and error messages.
        /// </summary>
        /// <param name="name">The name of the command.</param>
        /// <param name="description">Description of the command. This is displayed when CLI help is requested.</param>
        /// <param name="logger">An ILogger instance.</param>
        protected CliCommand(string name, string description, ILogger? logger)
            : base(name, description)
        {
            this._logger = logger;
        }

        /// <summary>
        /// Execute a task for a command and get the correct exit code.
        /// </summary>
        /// <param name="commandTask">The Task to execute for the command.</param>
        /// <param name="successMessage">The success message to display for console output when the task is successful.</param>
        /// <returns>The exit code for this command's results.</returns>
        protected async Task<int> ExecuteWithExitCodeAsync(Task commandTask, string? successMessage = null)
        {
            commandTask.ThrowIfNull(nameof(commandTask));

            try
            {
                if (_logger != null && _logger.IsEnabled(LogLevel.Debug))
                {
                    _logger.LogDebug("Invoking command {0}", this.GetType().Name);
                }

                await commandTask.ConfigureAwait(false);
            }
            catch (Exception e)
            {
                _logger?.LogError(e, $"{e.Message}");
                return (int)1;
            }

            if (!string.IsNullOrWhiteSpace(successMessage))
            {
                _logger?.LogInformation(successMessage);
            }

            return (int)0;
        }
    }
}
