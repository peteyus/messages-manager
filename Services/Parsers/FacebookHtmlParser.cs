namespace Services.Parsers
{
    using Core.Extensions;
    using Core.Interfaces;
    using Core.Models;
    using HtmlAgilityPack;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO.Abstractions;
    using System.Text.RegularExpressions;

    public class FacebookHtmlParser : IMessageParser
    {
        public IEnumerable<Message> ReadMessagesFromFile(string sourceFilePath, MessageParserConfiguration? options = null)
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.Load(sourceFilePath);// TODO PRJ: Test here? tighten the try? Submethod for this logic?
            var messageRootNode = htmlDoc.DocumentNode.SelectSingleNode("//div[@role='main']");
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
            // They are not consistent - 2023-11-15
            var messageRootNode = htmlDoc.DocumentNode.SelectSingleNode("//div[@role='main']");
            var messageNodes = messageRootNode.Elements("div").Where(node => node.Attributes.AttributesWithName("class").Any(attr => attr.Value.Contains("_2lej")));

            foreach (HtmlNode node in messageNodes)
            {
                var divs = node.Elements("div");

                var message = new Message();
                var personNode = divs.First(node => node.Attributes["class"].Value.Contains("_2lek"));
                message.Sender = new Person { DisplayName = personNode.InnerText }; // TODO PRJ: some sort of person lookup/matching here, eventually. New service.

                var timestampNode = divs.First(node => node.Attributes["class"].Value.Contains("_2lem"));
                this.ProcessTimeStamp(message, timestampNode, TimeZoneInfo.Local); // TODO PRJ: Make timezone of static HTML configurable

                var contentNode = divs.First(node => node.Attributes["class"].Value.Contains("_2let"));
                this.PopulateMessageContent(message, contentNode);
                messages.Add(message);
            }

            return messages;
        }

        private void PopulateMessageContentRecursive(Message message, HtmlNode contentNode)
        {
            foreach (var node in contentNode.ChildNodes.Where(node => node.NodeType == HtmlNodeType.Element))
            {
                this.DetectContentAndPopulateMessage(message, node, 0);
            }
        }

        private void DetectContentAndPopulateMessage(Message message, HtmlNode node, int depth)
        {
            var childElements = node.ChildNodes.Where(node => node.NodeType == HtmlNodeType.Element);
            if (childElements.Any(node => node.Name == "div"))
            {
                foreach (var childNode in node.Elements("div"))
                {
                    this.DetectContentAndPopulateMessage(message, childNode, depth + 1);
                }
            }

            foreach (var child in childElements.Where(node => node.Name != "div"))
            {
                switch (child.Name)
                {
                    case "img":
                        this.ProcessImage(message, child);
                        break;
                    case "audio":
                        this.ProcessAudio(message, child);
                        break;
                    case "a":
                        if (child.InnerText == node.InnerText && !node.Elements("img").Any())
                        {
                            this.ProcessLink(message, child);
                        }
                        else if (node.Elements("img").Any())
                        {
                            foreach (var imageNode in node.Elements("img"))
                            {
                                this.ProcessImage(message, imageNode);
                            }
                        }
                        else
                        {
                            this.ProcessText(message, child);
                        }
                        break;
                    default:
                        this.ProcessText(message, child);
                        break;
                }
            }

            if (node.InnerText.HasValue())
            {
                this.ProcessText(message, node);
            }
        }

        private void ProcessTimeStamp(Message message, HtmlNode timestampNode, TimeZoneInfo defaultTimeZone)
        {
            var timestampText = timestampNode.InnerText;
            if (Regex.IsMatch(timestampText, @"(Z|[+-]\d{2}:\d{2})$"))
            {
                message.Timestamp = DateTimeOffset.Parse(timestampText, CultureInfo.InvariantCulture);
                return;
            }

            var dateTime = DateTime.Parse(timestampText, CultureInfo.InvariantCulture);
            message.Timestamp = new DateTimeOffset(dateTime, defaultTimeZone.GetUtcOffset(dateTime));
        }

        private void ProcessImage(Message message, HtmlNode imageNode)
        {
            // TODO PRJ: Import images into data store? Or reference on disk?
            message.Images.Add(new Photo { ImageUrl = imageNode.Attributes["src"].Value, CreatedAt = message.Timestamp });
        }

        private void ProcessAudio(Message message, HtmlNode audioNode)
        {
            // TODO PRJ: Import audio into data store? Or reference on disk?
            message.Audio.Add(new Audio { FileUrl = audioNode.Attributes["src"].Value, CreatedAt = message.Timestamp });
        }

        private void ProcessLink(Message message, HtmlNode linkNode)
        {
            string uriString = linkNode.GetAttributeValue("href", "");
            if (uriString.HasValue())
            {
                var newUri = new Uri(uriString);
                if (!message.Links.Contains(newUri))
                {
                    message.Links.Add(newUri);
                }
            }
        }

        private void ProcessText(Message message, HtmlNode textNode)
        {
            if (message.MessageText.HasValue())
            {
                throw new Exception("Message already has text. I need to decide how to handle this.");
            }

            this.FindInnerText(message, textNode);
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
