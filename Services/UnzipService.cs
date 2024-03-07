using Core;
using Core.Extensions;
using Core.Interfaces;
using Core.Models.Application;
using System.IO.Abstractions;
using System.IO.Compression;

namespace Services
{
    public class UnzipService : IUnzipService
    {
        // TODO PRJ: Make this configurable? Don't sweat it? Assumes 50% compression on a 10GB zip file which is pretty atypical anyway.
        private const long MaxSize = 20L * 1024 * 1024 * 1024 * 1024;
        private readonly IFileSystem fileSystem;

        public UnzipService(IFileSystem fileSystem)
        {
            fileSystem.ThrowIfNull(nameof(fileSystem));
            this.fileSystem = fileSystem;
        }

        public Folder UnzipFile(string zipFilePath, string? destination = null)
        {
            this.CheckMaxUnzippedSize(zipFilePath);

            destination ??= GetTemporaryDirectory();
            ZipFile.ExtractToDirectory(zipFilePath, destination);

            return BuildDirectoryStructure(destination);
        }

        private bool CheckMaxUnzippedSize(string zipFilePath)
        {
            if (!this.fileSystem.File.Exists(zipFilePath))
            {
                throw new FileNotFoundException(zipFilePath);
            }

            using var zipFile = ZipFile.OpenRead(zipFilePath);

            // Quickly check the value from the zip header
            var declaredSize = zipFile.Entries.Sum(entry => entry.Length);
            if (declaredSize > MaxSize)
                throw new Exception(Strings.ErrorFileExceedsMaxAllowedSzie.FormatCurrentCulture(this.fileSystem.Path.GetFileName(zipFilePath)));

            // Sum the sizes of the zip entries
            long totalSize = 0;
            foreach (var entry in zipFile.Entries)
            {
                totalSize += entry.Length;
                if (totalSize >= MaxSize)
                    throw new Exception(Strings.ErrorFileExceedsMaxAllowedSzie.FormatCurrentCulture(this.fileSystem.Path.GetFileName(zipFilePath)));
            }

            return true;
        }

        private string GetTemporaryDirectory()
        {
            string tempDirectory = this.fileSystem.Path.Combine(
                this.fileSystem.Path.GetTempPath(),
                this.fileSystem.Path.GetRandomFileName());

            if (this.fileSystem.Path.Exists(tempDirectory))
            {
                return GetTemporaryDirectory();
            }
            else
            {
                Directory.CreateDirectory(tempDirectory);
                return tempDirectory;
            }
        }

        // TODO PRJ: Protect against very large file structures? Can zips unzip symlinks?
        private Folder BuildDirectoryStructure(string path)
        {
            var root = new Folder
            {
                Name = this.fileSystem.Path.GetDirectoryName(path),
                Path = path
            };

            foreach (var directory in this.fileSystem.Directory.GetDirectories(path))
            {
                root.Folders.Add(this.BuildDirectoryStructure(directory));
            }

            foreach (var file in this.fileSystem.Directory.GetFiles(path))
            {
                var info = new FileInfo(file);
                root.Files.Add(new Core.Models.Application.File
                {
                    Name = info.Name,
                    Path = info.FullName,
                    Size = info.Length
                });
            }

            return root;
        }
    }
}
