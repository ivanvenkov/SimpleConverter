using jConverter.Application.Managers;
using jConverter.Application.Models;
using Microsoft.AspNetCore.Mvc;

namespace JConverter.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConverterApiController : ControllerBase
    {
        private readonly IFileManager fileManager;

        public ConverterApiController(IFileManager fileManager)
            => this.fileManager = fileManager;

        /// <summary>
        /// Uploads files to the file system.
        /// </summary>
        [HttpPost("upload-file")]
        public async Task<ActionResult> UploadFile([FromForm] InputRequestModel request)
        {
            var convertedFile = await this.fileManager.Upload(request);
            return StatusCode(201, convertedFile);
        }
    }
}