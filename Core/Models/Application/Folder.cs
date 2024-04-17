namespace Core.Models.Application
{
    public class Folder
    {
        public Folder()
        {
            this.Folders = [];
            this.Files = [];
        }

        public string? Name { get; set; }
        public string? Path { get; set; }
        public IList<Folder> Folders { get; }
        public IList<File> Files { get; }
    }
}
