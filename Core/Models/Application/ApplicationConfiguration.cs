namespace Core.Models.Application
{
    // TODO PRJ: Deal with nulls better than suppressing or constant null checking.
    public class ApplicationConfiguration
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public ElasticConfiguration Elastic { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    }
}
