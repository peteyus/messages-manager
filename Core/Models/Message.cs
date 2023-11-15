namespace Core.Models
{
    public class Message
    {
        public Message()
        {
            this.Reactions = new List<MessageReaction>();
            this.Images = new List<Photo>();
            this.Audio = new List<Audio>();
        }

        // TODO PRJ: Enum? Want to be able to differentiate
        public string? Source { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public Person? Sender { get; set; }
        public string? MessageText { get; set; }
        public IList<Photo> Images { get; set; }
        public IList<Audio> Audio { get; set; }
        public IList<MessageReaction> Reactions { get; }
    }
}
