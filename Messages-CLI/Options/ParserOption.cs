using Core.Enums;
using System.CommandLine;

namespace Messages.CLI.Options
{
    internal static class ParserOptions
    {
        internal static Option<MessageParsers> ParserOption = new Option<MessageParsers>(
            name: "--parser",
            description: "Manually specify the parser to use.",
            parseArgument: result =>
            {
                if (result.Tokens.Count == 0)
                {
                    return MessageParsers.Unknown;
                }

                if (Enum.TryParse(result.Tokens.Single().Value, out MessageParsers value))
                {
                    return value;
                }

                return MessageParsers.Unknown;
            });
    }
}
