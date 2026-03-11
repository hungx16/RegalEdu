using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;

namespace RegalEdu.Api.Controllers
{
    [Authorize]
    public class FileController : BaseController
    {
        private readonly IRegalEducationDbContext _context;
        private readonly IFileService _fileService;
        private readonly ILocalizationService _localizer;
        public FileController(
            IMediator mediator,
            IRegalEducationDbContext context,
            IFileService fileService,
            ILocalizationService localizer) : base(mediator)
        {
            _context = context;
            _fileService = fileService;
            _localizer = localizer;
        }

        [HttpGet("download")]
        public async Task<IActionResult> Download([FromQuery] string file)
        {
            var (fileContents, contentType, fileName) = await _fileService.DownloadFileAsync(file);

            if (fileContents == null)
                return NotFound();

            // Trả trực tiếp file nhị phân, browser sẽ tự xử lý tải về
            return File(fileContents, contentType ?? "application/octet-stream", fileName);
        }
        [AllowAnonymous]
        [HttpGet("download-by-attachment")]
        public async Task<IActionResult> DownloadByAttachmentId([FromQuery] string id, CancellationToken ct = default)
        {
            if (!Guid.TryParse(id, out var attachmentId))
                return BadRequest();

            var attachment = await _context.Attachments
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == attachmentId, ct);

            if (attachment == null || string.IsNullOrWhiteSpace(attachment.Path))
                return NotFound();

            var (fileContents, contentType, fileName) = await _fileService.DownloadFileAsync(attachment.Path);

            if (fileContents == null)
                return NotFound();

            return File(fileContents, contentType ?? "application/octet-stream", fileName);
        }

        public record UploadedFileDto(string RelativePath, string FileName, long Size, string? ContentType);

        // Upload TEMP (chung 1 thư mục temp)
        [HttpPut("temp")]
        [Consumes("multipart/form-data")]
        [RequestFormLimits(MultipartBodyLengthLimit = 50_000_0000)]
        [RequestSizeLimit(50_000_0000)]
        [AllowAnonymous]
        public async Task<ActionResult> UploadTemp([FromForm] List<IFormFile> files, CancellationToken ct = default)
        {
            if (files is null || files.Count == 0)
                return Ok(Result.Failure(_localizer["NoFiles"]));

            var list = new List<UploadedFileDto>();
            foreach (var f in files)
            {
                try
                {
                    // Lưu thẳng vào temp/, FileService tự tạo tên GUID.ext => duy nhất
                    var rel = await _fileService.UploadFileAsync(f, "temp");
                    list.Add(new UploadedFileDto(rel, Path.GetFileName(rel), f.Length, f.ContentType));
                }
                catch
                {
                    return Ok(Result.Failure(_localizer["ERR_FILE_UPLOAD_FAILED"]));
                }
            }
            return Ok(Result.Success(list));
        }

        // Xoá TEMP
        [HttpDelete("temp")]
        public async Task<ActionResult> DeleteTemp([FromQuery] string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return Ok(Result.Failure(_localizer["NoFilePath"]));

            var ok = await _fileService.DeleteFileAsync(path);
            return ok ? Ok(Result.Success()) : Ok(Result.Failure(_localizer["FileNotFound"]));
        }
    }
}
