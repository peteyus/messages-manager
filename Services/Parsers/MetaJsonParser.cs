namespace Services.Parsers
{
    using Core.Enums;
    using Core.Extensions;
    using Core.Interfaces;
    using Core.Models;
    using System.IO.Abstractions;
    using System.Text;
    using System.Text.Json.Nodes;
    using System.Text.RegularExpressions;

    public abstract class MetaJsonParser : IMessageParser
    {
        private readonly IFileSystem fileSystem;

        public MetaJsonParser(IFileSystem fileSystem)
        {
            fileSystem.ThrowIfNull(nameof(fileSystem));

            this.fileSystem = fileSystem;
        }

        public abstract string Source { get; }

        public abstract MessageParsers ParserType { get; }

        public MessageSample ConfigureParsingAndReturnSample(string sourceFilePath, MessageParserConfiguration? options = null)
        {
            var messageContent = this.LoadJsonFromFile(sourceFilePath);
            var parsedBody = JsonNode.Parse(messageContent);
            var messageNodes = parsedBody!["messages"]?.AsArray() ?? new JsonArray();
            var messageNode = messageNodes.Count() > 0 ? messageNodes[0] : null;
            var message = new Message();
            if (messageNode is not null)
            {
                message = this.ProcessSingleMessage(messageNode);
            }

            options ??= new MessageParserConfiguration { Parser = ParserType };
            return new MessageSample { SampleMessage = message, ParserConfiguration = options };
        }

        public IEnumerable<Message> ReadMessages(string messageContent, MessageParserConfiguration? options = null)
        {
            var messages = new List<Message>();

            var parsedBody = JsonNode.Parse(messageContent);
            var messageNodes = parsedBody!["messages"]?.AsArray() ?? new JsonArray();
            foreach (var messageNode in messageNodes)
            {
                if (messageNode is null) continue;

                var message = this.ProcessSingleMessage(messageNode);
                messages.Add(message);
            }

            return messages;
        }

        public IEnumerable<Message> ReadMessagesFromFile(string sourceFilePath, MessageParserConfiguration? options = null)
        {
            var messageContent = this.LoadJsonFromFile(sourceFilePath);
            return this.ReadMessages(messageContent, options);
        }

        private string LoadJsonFromFile(string filePath)
        {
            if (!this.fileSystem.File.Exists(filePath))
            {
                throw new FileNotFoundException(Core.Strings.ErrorFileNotFound.FormatCurrentCulture(filePath));
            }

            return this.fileSystem.File.ReadAllText(filePath);
        }

        private Message ProcessSingleMessage(JsonNode messageNode)
        {
            var message = new Message();

            message.Sender = new Person { DisplayName = messageNode["sender_name"]?.AsValue().ToString() };

            this.ProcessTimestamp(message, messageNode["timestamp_ms"]);
            this.ProcessText(message, messageNode["content"]);
            this.ProcessLinks(message, messageNode["content"]);
            this.ProcessVideos(message, messageNode["videos"]?.AsArray());
            this.ProcessAudio(message, messageNode["audio_files"]?.AsArray());
            this.ProcessImages(message, messageNode["photos"]?.AsArray());
            this.ProcessReactions(message, messageNode["reactions"]?.AsArray());
            this.ProcessSharedContent(message, messageNode["shared"]);

            message.Source = this.Source;

            return message;
        }

        private string DecodeEmojiInString(string input)
        {
            Encoding targetEncoding = Encoding.GetEncoding("ISO-8859-1");
            var unescapeText = System.Text.RegularExpressions.Regex.Unescape(input);

            return Encoding.UTF8.GetString(targetEncoding.GetBytes(unescapeText));
        }

        private void ProcessAudio(Message message, JsonArray? audioNodes)
        {
            if (audioNodes is null) return;

            foreach (var audioNode in audioNodes)
            {
                if (audioNode is null) continue;
                message.Audio.Add(new Audio
                {
                    FileUrl = audioNode["uri"]?.ToString(),
                    CreatedAt = DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(audioNode["creation_timestamp"]?.ToString() ?? "0"))
                });
            }
        }

        private void ProcessImages(Message message, JsonArray? imageNodes)
        {
            if (imageNodes is null) return;

            foreach (var imageNode in imageNodes)
            {
                if (imageNode is null) continue;
                message.Images.Add(new Photo
                {
                    ImageUrl = imageNode["uri"]?.ToString(),
                    CreatedAt = DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(imageNode["creation_timestamp"]?.ToString() ?? "0"))
                });
            }
        }

        private void ProcessLinks(Message message, JsonNode? textNode)
        {
            if (textNode is null) return;

            var genericLinkDetection = "(?:(http|https)\\:\\/\\/)?[a-zA-Z0-9\\-\\.]+\\.[a-zA-Z]{2,3}(\\/\\S*)?";
            var specificLinkDetection = "(?:(http|https)\\:\\/\\/)[a-zA-Z0-9\\-\\.]+\\.[a-zA-Z0-9]{2,3}(\\/\\S*)?";

            var genericLinks = Regex.Matches(textNode.ToString(), genericLinkDetection);
            var specificLinks = Regex.Matches(textNode.ToString(), specificLinkDetection);

            foreach (string link in genericLinks)
            {
                var assumedLink = link;
                if (!link.StartsWith("http"))
                {
                    assumedLink = $"https://{link}";
                }

                var uri = new Uri(assumedLink);
                if (!message.Links.Contains(uri))
                {
                    message.Links.Add(uri);
                }
            }

            foreach (string link in specificLinks)
            {
                var uri = new Uri(link);
                if (!message.Links.Contains(uri))
                {
                    message.Links.Add(uri);
                }
            }
        }

        private void ProcessReactions(Message message, JsonArray? reactionsNode)
        {
            if (reactionsNode is null) return;

            foreach (var reactionNode in reactionsNode)
            {
                if (reactionNode is null) continue;
                var value = reactionNode["reaction"]?.ToString().Trim();
                if (value is null) continue;

                var unicodeString = this.DecodeEmojiInString(value);
                var reaction = new Reaction { ReactionText = unicodeString, Person = new Person { DisplayName = reactionNode["actor"]?.AsValue().ToString() } };
                message.Reactions.Add(reaction);
            }
        }

        private void ProcessSharedContent(Message message, JsonNode? sharedContentNode)
        {
            if (sharedContentNode is null) return;

            var share = new Share();
            var linkNode = sharedContentNode["link"];
            var ownerNode = sharedContentNode["original_content_owner"];
            var textNode = sharedContentNode["share_text"];

            // messages with something that looks like a link but isn't a link generate a share node with a copy of the body text. We don't need to preserve that.
            if (textNode is not null && linkNode is null && ownerNode is null)
            {
                return;
            }

            share.Url = linkNode?.AsValue().ToString();
            share.OriginalContentOwner = ownerNode?.AsValue().ToString();
            share.ShareText = textNode?.AsValue().ToString();

            message.Share = share;
        }

        private void ProcessText(Message message, JsonNode? textNode)
        {
            if (textNode is null)
            {
                return;
            }

            if (message.MessageText.HasValue())
            {
                throw new Exception("Message already has text. I need to decide how to handle this.");
            }

            message.MessageText = this.DecodeEmojiInString(textNode.ToString());
        }

        private void ProcessTimestamp(Message message, JsonNode? timestampNode)
        {
            if (timestampNode == null) return;
            bool timestampParse = Int64.TryParse(timestampNode?.AsValue().ToString(), out long timestamp);
            if (!timestampParse)
            {
                throw new Exception("Failed to parse timestamp."); // TODO PRJ: Resource
            }

            message.Timestamp = DateTimeOffset.FromUnixTimeMilliseconds(timestamp);
        }

        private void ProcessVideos(Message message, JsonArray? videosNode)
        {
            if (videosNode is null) return;

            // TODO PRJ: Import images into data store? Or reference on disk?
            foreach (var videoNode in videosNode)
            {
                if (videoNode is null) continue;

                message.Videos.Add(new Video { VideoUrl = videoNode["uri"]?.ToString() });
            }
        }
    }
}
