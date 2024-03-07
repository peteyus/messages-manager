using Core.Models.Application;

namespace Core.Interfaces
{
    public interface IUnzipService
    {
        public Folder UnzipFile(string zipFilePath, string? destination = null);
    }
}
