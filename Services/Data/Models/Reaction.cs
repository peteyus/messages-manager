namespace Services.Data.Models
{
    public class Reaction
    {
        public int Id { get; set; }
        public int PersonId { get; set; }
        public string? ReactionText { get; set; }
        public Person? Person { get; set; }
    }
}
