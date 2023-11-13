namespace Services.Parsers
{
    using Core.Extensions;
    using Core.Interfaces;
    using Core.Models;
    using HtmlAgilityPack;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO.Abstractions;

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

            // TODO PRJ: Going to need to get configuration after we read the file probably.
            // For now assume same format. Neeed to configure class names? or are these consistent?
            var messageRootNode = htmlDoc.DocumentNode.SelectSingleNode("div[@role='main']");
            var messageNodes = messageRootNode.Elements("div").Where(node => node.Attributes.AttributesWithName("class").Any(attr => attr.Value.Contains("_2lej")));

            foreach (HtmlNode node in messageNodes)
            {
                var divs = node.Elements("div");

                var message = new Message();
                var personNode = divs.First(node => node.Attributes["class"].Value.Contains("_2lek"));
                message.Sender = new Person { DisplayName = personNode.InnerText }; // TODO PRJ: some sort of person lookup/matching here, eventually. New service.

                var timestampNode = divs.First(node => node.Attributes["class"].Value.Contains("_2lem"));
                message.Timestamp = DateTime.Parse(timestampNode.InnerText);

                var contentNode = divs.First(node => node.Attributes["class"].Value.Contains("_2let"));
                this.PopulateMessageContent(message, contentNode);
                messages.Add(message);
            }

            return messages;
        }

        private void PopulateMessageContent(Message message, HtmlNode contentNode)
        {
            var imageNodes = contentNode.Descendants("img");
            foreach (var imageNode in imageNodes)
            {
                message.ImageUrls.Add(imageNode.Attributes["src"].Value); // TODO PRJ: Import images into data store? Or reference on disk?
            }

            this.FindInnerText(message, contentNode);
        }

        private void FindInnerText(Message message, HtmlNode parentNode)
        {
            if (parentNode.InnerText.HasValue())
            {
                this.GetMessageTextWithReactions(message, parentNode, parentNode.InnerText.Trim());
                return;
            }

            foreach (var node in parentNode.ChildNodes)
            {
                if (node.InnerText.HasValue())
                {
                    this.GetMessageTextWithReactions(message, node, node.InnerText.Trim());
                    return;
                }
                else
                {
                    this.FindInnerText(message, node);
                }
            }
        }

        private void GetMessageTextWithReactions(Message message, HtmlNode nodeWithText, string text)
        {
            var reactionListNode = nodeWithText.Descendants("ul").FirstOrDefault();
            if (reactionListNode != null)
            {
                var reactionText = reactionListNode.InnerText.Trim();
                if (text.Contains(reactionText))
                {
                    text = text.Substring(0, text.LastIndexOf(reactionText));
                }

                var childNodes = reactionListNode.Elements("li");
                foreach (var reactionNode in childNodes)
                {
                    var unicodeString = new StringInfo(reactionNode.InnerText.Trim());
                    var reaction = new MessageReaction { Reaction = unicodeString.SubstringByTextElements(0, 1), Person = new Person { DisplayName = unicodeString.SubstringByTextElements(1) } };
                    message.Reactions.Add(reaction);
                }
            }

            message.MessageText = text.Trim();
        }
    }
}
