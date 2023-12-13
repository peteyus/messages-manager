namespace Services.Data.Models
{
    public class MessageReaction
    {
        public int Id { get; set; }
        public int PersonId { get; set; }
        public string? Reaction { get; set; }
        public Person? Person { get; set; }
    }
}
