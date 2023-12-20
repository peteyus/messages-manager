namespace Services.Data.Models
{
    using Services.Data.Models.Equality;
    using System.Collections.Generic;

    public class Message
    {
        public Message()
        {
            var comparer = new DataEqualityComparer();

            this.Reactions = new HashSet<Reaction>(comparer);
            this.Images = new HashSet<Photo>(comparer);
            this.Audio = new HashSet<Audio>(comparer);
            this.Links = new HashSet<Uri>();
            this.Videos = new HashSet<Video>(comparer);
        }

        public int Id { get; set; }
        public int SenderId { get; set; }
        public int? ShareId { get; set; }
        public string? Source { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public Person? Sender { get; set; }
        public string? MessageText { get; set; }
        public string? MessageHtml { get; set; }
        public ICollection<Photo> Images { get; set; }
        public ICollection<Audio> Audio { get; set; }
        public ICollection<Uri> Links { get; set; }
        public ICollection<Video> Videos { get; set; }
        public Share? Share { get; set; }
        public ICollection<Reaction> Reactions { get; set; }
        public int ConversationId { get; set; }
    }
}
