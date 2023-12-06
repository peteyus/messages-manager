namespace Services.Data.Stores
{
    using Core.Extensions;
    using Microsoft.EntityFrameworkCore;
    using System.IO.Abstractions;

    public class SqliteContext : BaseDbContext
    {
        private readonly IFileSystem fileSystem;
        private string _databasePath;

        public SqliteContext(IFileSystem fileSystem)
        {
            fileSystem.ThrowIfNull(nameof(fileSystem));
            this.fileSystem = fileSystem;

            this._databasePath = this.GetDatabasePath();
        }

        public SqliteContext(IFileSystem fileSystem, DbContextOptions options) : base(options)
        {
            fileSystem.ThrowIfNull(nameof(fileSystem));
            this.fileSystem = fileSystem;

            this._databasePath = this.GetDatabasePath();
        }

        // TODO PRJ: app.config setting for this, along with selecting a preferred context.
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder.UseSqlite($"Data Source={_databasePath}");

        private string GetDatabasePath()
        {
            var folder = Environment.SpecialFolder.LocalApplicationData;
            var path = Environment.GetFolderPath(folder);
            return this.fileSystem.Path.Join(path, "messages-manager.db");
        }
    }
}
