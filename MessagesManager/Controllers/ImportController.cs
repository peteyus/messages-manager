namespace MessagesManager.Controllers
{
    using Core.Extensions;
    using Core.Interfaces;
    using Core.Models;
    using Microsoft.AspNetCore.Mvc;
    using System.IO.Abstractions;
    using System.Linq.Expressions;
    using System.Net.Http.Headers;
    using System.Text.Json;
    using System.Text.Json.Serialization;

    [ApiController]
    [Route("api/[controller]")]
    public class ImportController : Controller
    {
        private IWebHostEnvironment environment;
        private readonly IFileSystem fileSystem;
        private readonly IMessageImporter messageImporter;
        private readonly string uploadRoot;
        

        public ImportController(
            IWebHostEnvironment environment,
            IFileSystem fileSystem,
            IMessageImporter messageImporter)
        {
            environment.ThrowIfNull(nameof(environment));
            fileSystem.ThrowIfNull(nameof(fileSystem));
            messageImporter.ThrowIfNull(nameof(messageImporter));

            this.environment = environment;
            this.fileSystem = fileSystem;
            this.messageImporter = messageImporter;
            this.uploadRoot = this.fileSystem.Path.Combine(this.fileSystem.Path.GetTempPath(), "Upload");
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

                string newPath = this.fileSystem.Path.Combine(this.uploadRoot, sessionId);
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

                // TODO PRJ: Strategy pattern - determine file type here? Or separate "upload" and "import"?
                return Ok(new { fileName });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // TODO PRJ: FilePath must be URL Encoded and relative?
        [HttpPost]
        [Route("PreviewImportFile")]
        public async Task<ObjectResult> PreviewImportAsync()
        {
            try
            {
                var form = await Request.ReadFormAsync(CancellationToken.None);
                var filePath = form["filePath"].FirstOrDefault();
                filePath.ThrowIfNull("File path is required.");

                string sessionId = form["sessionId"].FirstOrDefault() ?? Guid.NewGuid().ToString();
                MessageParserConfiguration? config = null;
                var configString = form["config"].FirstOrDefault();
                if (configString.HasValue())
                {
                    config = JsonSerializer.Deserialize<MessageParserConfiguration>(configString);
                }
                
                var fullFilePath = this.fileSystem.Path.Combine(this.uploadRoot, sessionId, filePath);
                var preview = messageImporter.PreviewFileImport(fullFilePath, config);
                return Ok(preview);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
