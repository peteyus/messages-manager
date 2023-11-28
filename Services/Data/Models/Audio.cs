namespace Services.Data.Models
{
    internal class Audio
    {
        public int Id { get; set; }
        public string? FileUrl { get; set; }
        public DateTimeOffset? CreatedAt { get; set; }
        public string? TranscribedText { get; set; }
    }
}
