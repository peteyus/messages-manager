using System.CommandLine;
using System.IO.Abstractions;

namespace Messages.CLI.Options
{
    internal static class FileOptions
    {
        internal static Option<IFileInfo?> BuildFileOption(IFileSystem fileSystem)
        {
            return new Option<IFileInfo?>(
            name: "--file",
            description: "The path to a file to interact with.",
            isDefault: true,
            parseArgument: result =>
            {
                if (result.Tokens.Count == 0)
                    return null;

                string? filePath = result.Tokens.Single().Value;
                if (!fileSystem.File.Exists(filePath))
                {
                    result.ErrorMessage = "The file does not exist.";
                    return null;
                }
                else
                {
                    return fileSystem.FileInfo.New(filePath);
                }
            });
        }
    }
}