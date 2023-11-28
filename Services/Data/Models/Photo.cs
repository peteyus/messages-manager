namespace Services.Data.Models
{
    internal class Photo
    {
        public int Id { get; set; }
        public string? ImageUrl { get; set; }
        public DateTimeOffset? CreatedAt { get; set; }
    }
}
