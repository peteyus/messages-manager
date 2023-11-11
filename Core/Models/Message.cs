namespace Core.Models
{
    public class Message
    {
        public Message()
        {
            this.Reactions = new List<MessageReaction>();
        }

        public string? MessageText { get; set; } // TODO PRJ: How is unicode/emoji handled? Different type than string? Just at the display layer?
        public string? ImageUrl { get; set; }
        public DateTime Timestamp { get; set; }
        public IEnumerable<MessageReaction> Reactions { get; }
        public Person? Sender { get; set; }
    }
}
