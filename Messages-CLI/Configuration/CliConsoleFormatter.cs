namespace Messages.CLI.Configuration
{
    namespace DOS.CLI.Logging
    {
        using System;
        using System.Diagnostics.CodeAnalysis;
        using System.IO;
        using Microsoft.Extensions.Logging;
        using Microsoft.Extensions.Logging.Abstractions;
        using Microsoft.Extensions.Logging.Console;
        using Microsoft.Extensions.Options;

        // Justification: Difficult to test custom ConsoleFormatter implementation
        [ExcludeFromCodeCoverage]
        public class CliConsoleFormatter : ConsoleFormatter
        {
            private CliConsoleFormatterOptions options;
            private readonly IDisposable? optionsReloadToken;

            public const string FormatterName = "CliConsoleFormatter";
            private const string LogLevelStart = "[";
            private const string LogLevelEnd = "]";
            private const string LogLevelPadding = ": ";

            public CliConsoleFormatter(IOptionsMonitor<CliConsoleFormatterOptions> options) : base(FormatterName)
            {
                this.optionsReloadToken = options.OnChange(this.ReloadLoggerOptions);
                this.options = options.CurrentValue;
            }

            private void ReloadLoggerOptions(CliConsoleFormatterOptions options)
            {
                this.options = options;
            }

            public override void Write<TState>(in LogEntry<TState> logEntry,
                IExternalScopeProvider? scopeProvider,
                TextWriter textWriter)
            {
                string message =
                    logEntry.Formatter(
                        logEntry.State, logEntry.Exception);

                if (message == null)
                {
                    return;
                }

                if (!this.options.ExcludeLogLevel)
                {
                    textWriter.Write(LogLevelStart);
                    textWriter.Write(GetLogLevelString(logEntry.LogLevel));
                    textWriter.Write(LogLevelEnd);
                    textWriter.Write(LogLevelPadding);
                }

                textWriter.WriteLine(message);
            }

            private static string GetLogLevelString(LogLevel logLevel)
            {
                return logLevel switch
                {
                    LogLevel.Trace => "trce",
                    LogLevel.Debug => "dbug",
                    LogLevel.Information => "info",
                    LogLevel.Warning => "warn",
                    LogLevel.Error => "fail",
                    LogLevel.Critical => "crit",
                    _ => throw new ArgumentOutOfRangeException(nameof(logLevel))
                };
            }
        }
    }

}
