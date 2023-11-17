namespace Services.Parsers
{
    using Core;
    using Core.Extensions;
    using Core.Interfaces;
    using Core.Models;
    using HtmlAgilityPack;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text.RegularExpressions;
    using System.Web;

    public abstract class MetaHtmlParser : IMessageParser
    {
        public abstract string SourceName { get; }

        public IEnumerable<Message> ReadMessagesFromFile(string sourceFilePath, MessageParserConfiguration? options = null)
        {
            var htmlDoc = this.LoadHtmlFromFile(sourceFilePath);

            return ParseMessagesFromHtml(htmlDoc, options as MetaHtmlParserConfiguration);
        }

        public IEnumerable<Message> ReadMessages(string messageContent, MessageParserConfiguration? options = null)
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(messageContent);

            return ParseMessagesFromHtml(htmlDoc, options as MetaHtmlParserConfiguration);
        }

        public MessageSample ConfigureParsingAndReturnSample(string sourceFilePath, MessageParserConfiguration? options = null)
        {
            var htmlDoc = this.LoadHtmlFromFile(sourceFilePath);

            if (options == null || options is not MetaHtmlParserConfiguration metaOptions)
            {
                metaOptions = this.GenerateConfigurationFromFile(htmlDoc);
            }

            var sample = new MessageSample();
            sample.SampleMessage = this.ParseSingleMessageFromHtml(htmlDoc, metaOptions);
            sample.ParserConfiguration = metaOptions;

            return sample;
        }

        private HtmlDocument LoadHtmlFromFile(string sourceFilePath)
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.Load(sourceFilePath);// TODO PRJ: Test here? tighten the try? Submethod for this logic?
            var messageRootNode = htmlDoc.DocumentNode.SelectSingleNode("//div[@role='main']");
            if (messageRootNode == null)
            {
                throw new Exception(Strings.ErrorRootNodeNotFound.FormatCurrentCulture(sourceFilePath));
            }

            return htmlDoc;
        }

        private IEnumerable<Message> ParseMessagesFromHtml(HtmlDocument htmlDoc, MetaHtmlParserConfiguration? options = null)
        {
            var messages = new List<Message>();

            if (options is null)
            {
                options = new MetaHtmlParserConfiguration();
            }

            var messageRootNode = htmlDoc.DocumentNode.SelectSingleNode("//div[@role='main']");
            var messageNodes = messageRootNode.Elements("div").Where(node =>
                node.Attributes.AttributesWithName("class").Any(attr => attr.Value.Contains(options.MessageHeaderIdentifer)));

            foreach (HtmlNode node in messageNodes)
            {
                var message = this.ProcessSingleMessageNode(node, options);
                messages.Add(message);
            }

            return messages;
        }

        private Message ParseSingleMessageFromHtml(HtmlDocument htmlDoc, MetaHtmlParserConfiguration options)
        {
            var messageRootNode = htmlDoc.DocumentNode.SelectSingleNode("//div[@role='main']");
            var messageNode = messageRootNode.Elements("div").Where(node =>
                node.Attributes.AttributesWithName("class").Any(attr => attr.Value.Contains(options.MessageHeaderIdentifer))).FirstOrDefault();

            if (messageNode is null)
            {
                throw new Exception(Strings.ErrorNoMessageNodeFound);
            }

            return this.ProcessSingleMessageNode(messageNode, options);
        }

        private Message ProcessSingleMessageNode(HtmlNode messageNode, MetaHtmlParserConfiguration options)
        {
            var divs = messageNode.Elements("div");

            var message = new Message();
            message.Source = this.SourceName;
            var personNode = divs.First(node => node.Attributes["class"].Value.Contains(options.SenderNodeIdentifier));
            message.Sender = new Person { DisplayName = personNode.InnerText }; // TODO PRJ: some sort of person lookup/matching here, eventually. New service.

            var timestampNode = divs.First(node => node.Attributes["class"].Value.Contains(options.TimestampNodeIdentifier));
            this.ProcessTimeStamp(message, timestampNode, TimeZoneInfo.Local); // TODO PRJ: Make timezone of static HTML configurable

            var contentNode = divs.First(node => node.Attributes["class"].Value.Contains(options.ContentNodeIdentifier));
            this.PopulateMessageContent(message, contentNode);

            return message;
        }

        private void PopulateMessageContent(Message message, HtmlNode contentNode)
        {
            /*
             * Structure:
             * <div class="_2lej"> <!-- contentNode -->
             *   <div> <!-- calling this contentDiv -->
             *     <div></div> <!-- always blank? -->
             *     <div>Text content goes here.</div>
             *     <div>Link, if any in content, OR nested shared object</div>
             *     <div>Always blank</div>
             *     <div>Images, Audio, or reactions. May not be present</div>
             *     <div>Reactions, if images or audio</div>
             *   </div>
             * </div>
             * 
             * Nested Shared Object format:
             * <div class="_2lej"> <!-- contentNode -->
             *   <div> <!-- contentDiv -->
             *     <div></div> <!-- still always blank -->
             *     <div></div> <!-- I think I've seen message content with shared content? But not often? -->
             *     <div>
             *       <div>
             *         <div>Shared content text</div>
             *         <div>Shared content owner</div>
             *         <div>Shared content link</div>
             *       </div>
             *     </div>
             *     <div></div> <!-- still always blank -->
             *     <div>Reactions, if any</div>
             *   </div>
             * </div>
             */
            var contentDiv = contentNode.Element("div");
            var children = contentDiv.Elements("div");
            for (int i = 0; i < children.Count(); i++)
            {
                switch (i)
                {
                    case 0:
                    case 3:
                        break;
                    case 1:
                        this.ProcessText(message, children.ElementAt(i));
                        break;
                    case 2:
                        var linkNode = children.ElementAt(i);
                        if (linkNode.Elements("div").Any())
                        {
                            this.ProcessSharedContent(message, linkNode.Element("div"));
                        }
                        else
                        {
                            foreach (var link in linkNode.Elements("a"))
                            {
                                this.ProcessLink(message, linkNode);
                            }
                        }
                        break;
                    default:
                        var mediaNode = children.ElementAt(i);
                        foreach (var imageNode in mediaNode.Descendants("img"))
                        {
                            this.ProcessImage(message, imageNode);
                        }

                        foreach (var audioNode in mediaNode.Descendants("audio"))
                        {
                            this.ProcessAudio(message, audioNode);
                        }

                        foreach (var reactionNode in mediaNode.Descendants("ul"))
                        {
                            this.ProcessReactions(message, reactionNode);
                        }
                        break;
                }
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

            if (textNode.InnerText.HasValue())
            {
                message.MessageText = HttpUtility.HtmlDecode(textNode.InnerText);
            }

            if (textNode.InnerHtml.HasValue())
            {
                message.MessageHtml = textNode.InnerHtml;
                foreach (var linkNode in textNode.Elements("a"))
                {
                    this.ProcessLink(message, linkNode);
                }
            }
        }

        private void ProcessReactions(Message message, HtmlNode reactionsNode)
        {
            var childNodes = reactionsNode.Elements("li");
            foreach (var reactionNode in childNodes)
            {
                var unicodeString = new StringInfo(reactionNode.InnerText.Trim());
                var reaction = new MessageReaction { Reaction = unicodeString.SubstringByTextElements(0, 1), Person = new Person { DisplayName = unicodeString.SubstringByTextElements(1) } };
                message.Reactions.Add(reaction);
            }
        }

        private void ProcessSharedContent(Message message, HtmlNode sharedContentNode)
        {
            var childDivs = sharedContentNode.Elements("div");
            var share = new Share();
            var linkNode = sharedContentNode.Descendants("a").First();
            switch (childDivs.Count())
            {
                case 3:
                    share.ShareText = HttpUtility.HtmlDecode(childDivs.ElementAt(0).InnerText);
                    share.OriginalContentOwner = childDivs.ElementAt(1).InnerText;
                    share.Url = linkNode.GetAttributeValue("href", "");
                    break;
                case 2:
                    share.OriginalContentOwner = childDivs.ElementAt(0).InnerText;
                    share.Url = linkNode.GetAttributeValue("href", "");
                    break;
                default:
                    share.Url = linkNode.GetAttributeValue("href", "");
                    break;
            }

            message.Share = share;
        }

        private MetaHtmlParserConfiguration GenerateConfigurationFromFile(HtmlDocument htmlDoc)
        {
            throw new NotImplementedException();
        }
    }
}
