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
        public async Task<ObjectResult> UploadFileAsync()
        {
            try
            {
                var form = await Request.ReadFormAsync(CancellationToken.None);
                var file = form.Files[0];
                string sessionId = form["sessionId"].FirstOrDefault() ?? Guid.NewGuid().ToString();
                bool.TryParse(form["overwrite"].FirstOrDefault(), out bool overwrite);

                string folderName = this.fileSystem.Path.Combine("Upload", sessionId);
                string uploadRoot = this.fileSystem.Path.GetTempPath();
                string newPath = this.fileSystem.Path.Combine(uploadRoot, folderName);
                if (!this.fileSystem.Directory.Exists(newPath))
                {
                    this.fileSystem.Directory.CreateDirectory(newPath);
                 }

                string fileName = "";
                if (file.Length > 0)
                {
                    fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName?.Trim('"') ?? Guid.NewGuid().ToString();
                    string fullPath = this.fileSystem.Path.Combine(newPath, fileName);
                    if (this.fileSystem.File.Exists(fullPath) && !overwrite)
                    {
                        return Conflict("The file already exists.");
                    }
                    using (var stream = new FileStream(fullPath, FileMode.Create)) 
                    {
                        file.CopyTo(stream);
                    }
                }

                // TODO PRJ: Strategy pattern - determine file type here
                return Ok(fileName);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
