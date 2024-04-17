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
        public List<Folder> Folders { get; }
        public List<File> Files { get; }
    }

    public class RootFolder : Folder
    {
        public RootFolder(Folder clone)
        {
            this.Folders.AddRange(clone.Folders);
            this.Files.AddRange(clone.Files);
            this.Name = clone.Name;
            this.Path = clone.Path;
        }

        public string? LocalPath { get; set; }
    }
}
