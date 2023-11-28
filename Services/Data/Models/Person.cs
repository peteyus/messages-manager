namespace Services.Data.Models
{
    internal class Person
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? DisplayName { get; set; }
        public string? ThumbnailUrl { get; set; }
        public string? ImageUrl { get; set; }
    }
}
