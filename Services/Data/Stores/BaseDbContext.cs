namespace Services.Data.Stores
{
    using Microsoft.EntityFrameworkCore;
    using Services.Data.Models;
    using Services.Interfaces;

    public abstract class BaseDbContext : DbContext, IMessageContext
    {
        public BaseDbContext()
        {
        }

        public BaseDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbContext DbContext => this;

        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Audio> Audio { get; set; }
        public DbSet<Photo> Images { get; set; }
        public DbSet<Person> People { get; set; }
        public DbSet<Share> Shares { get; set; }
        public DbSet<Video> Videos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // TODO PRJ: Do I need to establish relationships? I can _probably_ just let EF convention do it. Research this
            modelBuilder.Entity<Conversation>().ToTable("Conversations");
            modelBuilder.Entity<Message>().ToTable("Messages");
            modelBuilder.Entity<Audio>().ToTable("Audio");
            modelBuilder.Entity<Photo>().ToTable("Images");
            modelBuilder.Entity<Person>().ToTable("People");
            modelBuilder.Entity<Share>().ToTable("Share"); // 1-1 with messages makes me think this could just go in the message table?
            modelBuilder.Entity<Video>().ToTable("Videos");

            base.OnModelCreating(modelBuilder);
        }
    }
}
