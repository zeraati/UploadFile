using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace UploadFile.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FileController : ControllerBase
    {
        [HttpPost("Upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            try
            {
                var folderName = Path.Combine(nameof(UploadFile));
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                if (file.Length > 0)
                {
                    var fileName = file.FileName;
                    var fullPath = Path.Combine(pathToSave, fileName);
                    var shortPath = Path.Combine(folderName, fileName);

                    using (var stream = new FileStream(fullPath, FileMode.OpenOrCreate))
                    {
                        file.CopyTo(stream);
                    }
                    return Ok(new { path=shortPath });
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }

        [HttpGet("DownloadContent/{FileName}")]
        public async Task<IActionResult> Download(string FileName= "family.json")
        {
            try
            {
                var folderName = Path.Combine(nameof(UploadFile));
                var pathToRead = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                var fileFullPath = Path.Combine(pathToRead,FileName);
                var readContent = System.IO.File.ReadAllText(fileFullPath);
                //var jsonContent = JArray.Parse(readContent);
                return Ok(new { content= readContent });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }
    }
}
