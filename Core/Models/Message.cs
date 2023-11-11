namespace Core.Models
{
    public class Message
    {
        public Message()
        {
            this.Reactions = new List<MessageReaction>();
            this.ImageUrls = new List<string>();
        }

        public string? MessageText { get; set; } // TODO PRJ: How is unicode/emoji handled? Different type than string? Just at the display layer?
        public IList<string> ImageUrls { get; set; }
        public DateTime Timestamp { get; set; }
        public IList<MessageReaction> Reactions { get; }
        public Person? Sender { get; set; }
        public string? Source { get; set; }
    }
}
