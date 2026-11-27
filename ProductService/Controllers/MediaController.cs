using CommonService.Annotations;
using CommonService.Interface;
using CommonService.Interface;
using Microsoft.AspNetCore.Mvc;
using CommonService.Filters;

namespace ProductService.Controllers;

/// <summary>
/// Upload ảnh/file lên MinIO và lấy về URL để gán vào ThumbnailUrl / ImageUrls của sản phẩm.
///
/// Luồng sử dụng:
///   1. Gọi POST /api/v1/media/upload → nhận URL
///   2. Dùng URL đó trong POST /api/v1/products (trường thumbnailUrl hoặc imageUrls)
/// </summary>
[ApiController]
[Route("api/v1/media")]
public class MediaController : ControllerBase
{
    private readonly IMediaService _mediaService;

    public MediaController(IMediaService mediaService)
    {
        _mediaService = mediaService;
    }

    // POST /api/v1/media/upload
    // Body: form-data, key = "file"
    [HttpPost("upload")]
    [RequiresPermission("POST", "/api/v1/media/upload")]
    [ApiMessage("Image uploaded successfully.")]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("File cannot be empty.");

        // Kiểm tra định dạng file được phép
        var allowedTypes = new[] { "image/jpeg", "image/png", "image/webp", "image/gif" };
        if (!allowedTypes.Contains(file.ContentType.ToLower()))
            return BadRequest("Only image files (jpg, png, webp, gif) are accepted.");

        // Upload vào temp/ — sẽ tự bị xóa sau 24h nếu không commit
        var tempUrl = await _mediaService.UploadFileAsync(file);
        return Ok(new { url = tempUrl });
    }

    // POST /api/v1/media/upload-multiple
    // Body: form-data, key = "files" (nhiều file)
    [HttpPost("upload-multiple")]
    [RequiresPermission("POST", "/api/v1/media/upload-multiple")]
    [ApiMessage("Images uploaded successfully.")]
    public async Task<IActionResult> UploadMultiple(IList<IFormFile> files)
    {
        if (files == null || files.Count == 0)
            return BadRequest("The file list cannot be empty.");

        if (files.Count > 10)
            return BadRequest("You can upload a maximum of 10 files at a time.");

        var allowedTypes = new[] { "image/jpeg", "image/png", "image/webp", "image/gif" };
        foreach (var f in files)
        {
            if (!allowedTypes.Contains(f.ContentType.ToLower()))
                return BadRequest($"File '{f.FileName}' has an invalid format. Only jpg, png, webp, and gif files are accepted.");
        }

        var tempUrls = await _mediaService.UploadMultipleAsync(files);
        return Ok(new { urls = tempUrls });
    }
}
