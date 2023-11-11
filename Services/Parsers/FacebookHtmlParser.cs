namespace Services.Parsers
{
    using Core.Extensions;
    using Core.Interfaces;
    using Core.Models;
    using System.Collections.Generic;
    using System.IO.Abstractions;

    public class FacebookHtmlParser : IMessageParser
    {
        private readonly IFileSystem fileSystem;

        public FacebookHtmlParser(IFileSystem fileSystem)
        {
            fileSystem.ThrowIfNull(nameof(fileSystem));

            this.fileSystem = fileSystem;
        }

        public IEnumerable<Message> ReadMessages()
        {
            throw new NotImplementedException();
        }
    }
}
