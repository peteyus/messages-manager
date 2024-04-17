using Core.Models.Application;

namespace Core.Interfaces
{
    public interface IUnzipService
    {
        public Folder UnzipFile(string zipFilePath, string? destination = null);

        /// <summary>
        /// Attempts to delete all temporary files this clsas has created.
        /// </summary>
        /// <returns>An awaitable task</returns>
        public Task CleanUpTempFiles();
    }
}
