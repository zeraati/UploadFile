using Microsoft.AspNetCore.Mvc;

namespace UploadFile.Controllers;
[ApiController]
[Route("[controller]")]
public class FileController : ControllerBase
{
	[HttpPost("[action]")]
	public async Task<IActionResult> Upload(string? FileName, IFormFile file)
	{
		try
		{
			var folderName = Path.Combine(nameof(UploadFile));
			var folderPath= Path.Combine(Directory.GetCurrentDirectory(), folderName);
			if (file.Length > 0)
			{
				if (string.IsNullOrEmpty(FileName) == false && FileName.ToUpper() == "GUID") FileName = Guid.NewGuid().ToString();
				var fileName = string.IsNullOrEmpty(FileName) ? file.FileName : FileName + Path.GetExtension(file.FileName);
				var fullPath = Path.Combine(folderPath, fileName);
				var shortPath = Path.Combine(folderName, fileName);

				if (System.IO.File.Exists(fullPath) == true) System.IO.File.Delete(fullPath);

				using (var stream = new FileStream(fullPath, FileMode.Create))
				{
					await file.CopyToAsync(stream);
				}
				return Ok(new { fileName });
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

	[HttpDelete("[action]/{FileName}")]
	public async Task<IActionResult> Delete(string FileName)
	{
		try
		{
			var folderName = Path.Combine(nameof(UploadFile));
			var folderPath = Path.Combine(Directory.GetCurrentDirectory(), folderName);
			var fullPath = Path.Combine(folderPath, FileName);
			if (System.IO.File.Exists(fullPath) == true) System.IO.File.Delete(fullPath);

			return Ok();
		}
		catch (Exception ex)
		{
			return StatusCode(500, $"Internal server error: {ex}");
		}
	}

	[HttpGet("[action]/{Filter}")]
	public async Task<IActionResult> GetFileNames(string Filter)
	{
		if (string.IsNullOrEmpty(Filter) == true) Filter = "*.*";

		try
		{
			var folderName = Path.Combine(nameof(UploadFile));
			var folderPath = Path.Combine(Directory.GetCurrentDirectory(), folderName);
			var filePaths = Directory.GetFiles(folderPath, Filter, SearchOption.TopDirectoryOnly);

			var result = filePaths.Select(x=>x.Replace(folderPath+"\\","")).ToList();

			return Ok(result);
		}
		catch (Exception ex)
		{
			return StatusCode(500, $"Internal server error: {ex}");
		}
	}

	[HttpGet("DownloadContent/{FileName}")]
	public async Task<IActionResult> Download(string FileName = "family.json")
	{
		try
		{
			var folderName = Path.Combine(nameof(UploadFile));
			var pathToRead = Path.Combine(Directory.GetCurrentDirectory(), folderName);
			var fullPath = Path.Combine(pathToRead, FileName);
			var readContent = await System.IO.File.ReadAllTextAsync(fullPath);
			return Ok(new { content = readContent });
		}
		catch (Exception ex)
		{
			return StatusCode(500, $"Internal server error: {ex}");
		}
	}

	[HttpGet("Download/{FileName}")]
	public async Task<IActionResult> DownloadFile(string FileName)
	{
		if (string.IsNullOrEmpty(FileName)) return BadRequest();

		try
		{
			var folderName = Path.Combine(nameof(UploadFile));
			var pathToRead = Path.Combine(Directory.GetCurrentDirectory(), folderName);
			var fullPath = Path.Combine(pathToRead, FileName);
			var extention = Path.GetExtension(FileName).Remove(0, 1);
			var fileByte = await System.IO.File.ReadAllBytesAsync(fullPath);

			switch (extention)
			{
				case "png":
					return File(fileByte, "image/png");
				case "jpg":
					return File(fileByte, "image/jpg");
				default:
					return File(fileByte, "image/jpg");
			}
		}

		catch (Exception ex)
		{
			return StatusCode(500, $"Internal server error: {ex}");
		}
	}
}
