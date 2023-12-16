namespace Services.Interfaces
{
    using Microsoft.EntityFrameworkCore;
    using Services.Data.Models;

    public interface IMessageContext
    {
        DbContext DbContext { get; }
        DbSet<Conversation> Conversations { get; set; }
        DbSet<Message> Messages { get; set; }
        DbSet<Audio> Audio { get; set; }
        DbSet<Photo> Images { get; set; }
        DbSet<Person> People { get; set; }
        DbSet<Share> Shares { get; set; }
        DbSet<Video> Videos { get; set; }
    }
}
