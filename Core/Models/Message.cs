﻿namespace Core.Models
{
    public class Message
    {
        public Message()
        {
            this.Reactions = new List<Reaction>();
            this.Images = new List<Photo>();
            this.Audio = new List<Audio>();
            this.Links = new List<Uri>();
            this.Videos = new List<Video>();
        }

        public int Id { get; set; }
        // TODO PRJ: Enum? Want to be able to differentiate
        public string? Source { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public Person? Sender { get; set; }
        public string? MessageText { get; set; }
        public string? MessageHtml { get; set; }
        public IList<Photo> Images { get; set; }
        public IList<Audio> Audio { get; set; }
        public IList<Uri> Links { get; set; }
        public IList<Video> Videos { get; set; }
        public Share? Share { get; set; }
        public IList<Reaction> Reactions { get; set; }
        public int ConversationId { get; set; }

        public override string ToString()
        {
            return $"Message, Sender {this.Sender?.DisplayName ?? "[empty]"} at Timestamp {this.Timestamp}.";
        }
    }
}
