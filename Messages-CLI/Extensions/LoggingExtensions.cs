using Core.Models.Application;
using Messages.CLI.Configuration;
using Messages.CLI.Configuration.DOS.CLI.Logging;
using Microsoft.Extensions.Logging;

namespace Messages.CLI.Extensions
{
    public static class LoggingExtensions
    {
        public static ILoggingBuilder AddConsoleFormatter(this ILoggingBuilder builder, Action<CliConsoleFormatterOptions> configure)
        {
            return builder.AddConsole(options => options.FormatterName = CliConsoleFormatter.FormatterName)
                .AddConsoleFormatter<CliConsoleFormatter, CliConsoleFormatterOptions>(configure);
        }
    }
}
