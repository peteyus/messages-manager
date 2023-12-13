namespace Services.Data.Models
{
    public class Audio
    {
        public int Id { get; set; }
        public string? FileUrl { get; set; }
        public DateTimeOffset? CreatedAt { get; set; }
        public string? TranscribedText { get; set; }
    }
}
