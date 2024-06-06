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

        /// <summary>
        /// Returns an <see cref="IMessageParser"/> which is capable of parsing the given <paramref name="config">.
        /// </summary>
        /// <param name="config">A <see cref="MessageParserConfiguration"/> that provides options for parsing.</param>
        /// <returns>An <see cref="IMessageParser"/> instance</returns>
        /// <throws><see cref="IllegalOperationException"/> if the parser is unknown or undetectable.</throws>
        IMessageParser GetParser(MessageParserConfiguration config);

        /// <summary>
        /// Returns an <see cref="IMessageParser"/> which is capable of parsing the given <paramref name="parserType">.
        /// </summary>
        /// <param name="parserType">A <see cref="MessageParsers"/> to attempt parsing.</param>
        /// <returns>An <see cref="IMessageParser"/> instance</returns>
        /// <throws><see cref="IllegalOperationException"/> if the parser is unknown or undetectable.</throws>
        IMessageParser GetParser(MessageParsers parserType);
    }
}
