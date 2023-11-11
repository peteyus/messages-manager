namespace Services.Parsers
{
    using Core.Extensions;
    using Core.Interfaces;
    using Core.Models;
    using HtmlAgilityPack;
    using System.Collections.Generic;
    using System.IO.Abstractions;
    using System.Xml.Linq;

    public class FacebookHtmlParser : IMessageParser
    {
        private readonly IFileSystem fileSystem;

        public FacebookHtmlParser(IFileSystem fileSystem)
        {
            fileSystem.ThrowIfNull(nameof(fileSystem));

            this.fileSystem = fileSystem;
        }

        public IEnumerable<Message> ReadMessagesFromFile(string sourceFilePath, MessageParserConfiguration? options = null)
        {
            if (!fileSystem.File.Exists(sourceFilePath))
            {
                throw new FileNotFoundException(Core.Strings.ErrorFileNotFound.FormatCurrentCulture(sourceFilePath));
            }

            var htmlDoc = new HtmlDocument();
            htmlDoc.Load(sourceFilePath);// TODO PRJ: Test here? tighten the try? Submethod for this logic?
            var messageRootNode = htmlDoc.DocumentNode.SelectSingleNode("div[@role='main']");
            if (messageRootNode == null)
            {
                throw new Exception(Core.Strings.ErrorRootNodeNotFound.FormatCurrentCulture(sourceFilePath));
            }

            return ParseMessagesFromHtml(htmlDoc);
        }

        public IEnumerable<Message> ReadMessages(string messageContent, MessageParserConfiguration? options = null)
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(messageContent);

            return ParseMessagesFromHtml(htmlDoc);
        }

        private IEnumerable<Message> ParseMessagesFromHtml(HtmlDocument htmlDoc)
        {
            var messages = new List<Message>();

            // TODO PRJ: Going to need to get configuration after we read the file probably. For now assume same format.
            var messageRootNode = htmlDoc.DocumentNode.SelectSingleNode("div[@role='main']");

            foreach (HtmlNode node in messageRootNode.SelectNodes("/div"))
            {
                var message = new Message();
                var personNode = node.SelectSingleNode("//div[@class='_2lek']");
                message.Sender = new Person { DisplayName = personNode.InnerText }; // TODO PRJ: some sort of person lookup/matching here, eventually. New service.

                var timestampNode = node.SelectSingleNode("//div[@class='_2lem']");
                message.Timestamp = DateTime.Parse(timestampNode.InnerText);

                var contentNode = node.SelectSingleNode("//div[@class='_2let']");
                this.PopulateMessageContent(message, contentNode);
                messages.Add(message);
            }

            return messages;
        }

        private void PopulateMessageContent(Message message, HtmlNode contentNode)
        {
            if (contentNode.InnerHtml.Contains("<img"))
            {
                var imageNodes = contentNode.SelectNodes("//img");
                foreach (var imageNode in imageNodes)
                {
                    message.ImageUrls.Add(imageNode.Attributes["src"].Value); // TODO PRJ: Import images into data store? Or reference on disk?
                }
            }

            message.MessageText = FindInnerText(contentNode);
        }

        private string FindInnerText(HtmlNode parentNode)
        {
            if (parentNode.InnerText.HasValue())
            {
                return parentNode.InnerText.Trim(); // TODO PRJ: Reactions are contained in the same root node. Test for them/exclude them? Find them separately?
            }

            foreach (var node in parentNode.ChildNodes)
            {
                if (node.InnerText.HasValue())
                {
                    return node.InnerText.Trim();
                }
                else
                {
                    return FindInnerText(node);
                }
            }

            return string.Empty;
        }
    }
}
