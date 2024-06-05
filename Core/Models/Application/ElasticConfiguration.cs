namespace Core.Models.Application
{
    public class ElasticConfiguration
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public string ApiKey { get; set; }
        public string ApiUrl { get; set; }
        public string IndexName { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    }
}
