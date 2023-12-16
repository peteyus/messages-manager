namespace Core.Models
{
    public class Reaction
    {
        public int Id { get; set; }
        public string? ReactionText { get; set; }
        public Person? Person { get; set; }
    }
}
