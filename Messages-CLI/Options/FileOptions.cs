using System.CommandLine;
using System.ComponentModel;
using System.IO.Abstractions;
using System.Runtime.CompilerServices;

namespace Messages.CLI.Options
{
    internal static class FileOptions
    {
        internal static Option<IFileInfo?> BuildFileOption(IFileSystem fileSystem)
        {
            return new Option<IFileInfo?>(
            name: "--file",
            description: "The path to a file to interact with.",
            parseArgument: result =>
            {
                if (result.Tokens.Count == 0)
                    return null;

                string? filePath = result.Tokens.Single().Value;
                if (!fileSystem.File.Exists(filePath))
                {
                    result.ErrorMessage = $"The file {filePath} does not exist.";
                    return null;
                }
                else
                {
                    return fileSystem.FileInfo.New(filePath);
                }
            });
        }

        internal static Option<IEnumerable<IFileInfo>> BuildMultiFileOption(IFileSystem fileSystem)
        {
            return new Option<IEnumerable<IFileInfo>>(
                aliases: new[] { "--file", "--files", "--filenames" },
                description: "The path to one ore more files to interact with.",
                parseArgument: result =>
                {
                    if (result.Tokens.Count == 0)
                        throw new ArgumentException("One or more file names must be provided.");

                    var files = new List<IFileInfo>();
                    foreach (var token in result.Tokens)
                    {
                        string filePath = token.Value;
                        if (!fileSystem.File.Exists(filePath))
                        {
                            throw new ArgumentException($"The file {filePath} does not exist.");
                        }
                        files.Add(fileSystem.FileInfo.New(filePath));
                    }

                    return files;
                });
        }
    }
}