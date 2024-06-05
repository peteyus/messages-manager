using Microsoft.Extensions.Logging.Console;

namespace Messages.CLI.Configuration
{
    public class CliConsoleFormatterOptions : ConsoleFormatterOptions
    {
        /// <summary>
        /// If true, exclude the log level from the message.
        /// </summary>
        public bool ExcludeLogLevel { get; set; }
    }
}
