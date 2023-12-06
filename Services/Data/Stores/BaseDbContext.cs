namespace Services.Data.Stores
{
    using Microsoft.EntityFrameworkCore;
    using Services.Data.Models;

    public abstract class BaseDbContext : DbContext
    {
        public BaseDbContext()
        {
        }

        public BaseDbContext(DbContextOptions options) : base(options)
        {
        }

        internal DbSet<Message> Messages { get; set; }
        internal DbSet<Audio> Audio { get; set; }
        internal DbSet<Photo> Images { get; set; }
        internal DbSet<Person> People { get; set; }
        internal DbSet<Share> Shares { get; set; }
        internal DbSet<Video> Videos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
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
