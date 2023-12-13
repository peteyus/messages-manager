namespace Services.Data.Models
{
    public class Photo
    {
        public int Id { get; set; }
        public string? ImageUrl { get; set; }
        public DateTimeOffset? CreatedAt { get; set; }
    }
}
