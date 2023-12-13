namespace Core.Interfaces
{
    using Core.Enums;
    using Core.Models;

    public interface IParserDetector
    {
        /// <summary>
        /// Attempts to detect which type of <see cref="IMessageParser"/> to use to read a given <paramref name="filePath"/>
        /// </summary>
        /// <param name="filePath">The path to the file to import. Throws <see cref="FileNotFoundException"/> if the file can't be loaded.</param>
        /// <returns>A <see cref="MessageParserConfiguration"/> with the detected <see cref="MessageParsers"/> specified.</returns>
        /// <throws><see cref="InvalidCastException"/> if the file type is unknown/undetectable</throws>
        MessageParserConfiguration DetectParser(string filePath);
    }
}
