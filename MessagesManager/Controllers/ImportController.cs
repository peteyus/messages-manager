namespace MessagesManager.Controllers
{
    using Core.Extensions;
    using Microsoft.AspNetCore.Mvc;
    using System.IO.Abstractions;
    using System.Linq.Expressions;
    using System.Net.Http.Headers;

    [ApiController]
    [Route("api/[controller]")]
    public class ImportController : Controller
    {
        private IWebHostEnvironment environment;
        private readonly IFileSystem fileSystem;

        public ImportController(IWebHostEnvironment environment, IFileSystem fileSystem)
        {
            environment.ThrowIfNull(nameof(environment));
            fileSystem.ThrowIfNull(nameof(fileSystem));

            this.environment = environment;
            this.fileSystem = fileSystem;
        }

        [HttpPost, DisableRequestSizeLimit]
        [Route("UploadFile")]
        public ObjectResult UploadFile()
        {
            try
            {
                var file = Request.Form.Files[0];
                string folderName = "Upload";
                string webRootPath = this.environment.WebRootPath;
                string newPath = this.fileSystem.Path.Combine(webRootPath, folderName);
                if (!this.fileSystem.Directory.Exists(newPath)) // TODO PRJ: session paths? How to prevent user collisions on filenames?
                {
                    this.fileSystem.Directory.CreateDirectory(newPath);
                }

                string fileName = "";
                if (file.Length > 0)
                {
                    fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName?.Trim('"') ?? Guid.NewGuid().ToString();
                    string fullPath = this.fileSystem.Path.Combine(newPath, fileName);
                    using (var stream = new FileStream(fullPath, FileMode.Create)) // TODO PRJ: How to handle existing files if not session paths?
                    {
                        file.CopyTo(stream);
                    }
                }

                return Ok(fileName);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
