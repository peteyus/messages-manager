namespace Services.Data.Stores
{
    using Microsoft.EntityFrameworkCore;

    public class SqliteContext : BaseDbContext
    {
        private string _databasePath;

        public SqliteContext(DbContextOptions options) : base(options)
        {
            var folder = Environment.SpecialFolder.LocalApplicationData;
            var path = Environment.GetFolderPath(folder);
            _databasePath = Path.Join(path, "messages-manager.db");
        }

        // TODO PRJ: app.config setting for this, along with selecting a preferred context.
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder.UseSqlite($"Data Source={_databasePath}");
    }
}
