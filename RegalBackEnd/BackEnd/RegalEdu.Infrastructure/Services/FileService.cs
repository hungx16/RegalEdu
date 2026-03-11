using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using RegalEdu.Application.Common.Interfaces;

namespace RegalEdu.Infrastructure.Services
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        // Sử dụng dependency injection để inject IWebHostEnvironment
        // IWebHostEnvironment giúp lấy đường dẫn tới các thư mục gốc của ứng dụng, ví dụ wwwroot
        public FileService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<string> UploadFileAsync(IFormFile file, string subDirectory)
        {
            // 1. Xác định đường dẫn thư mục lưu file
            // Path.Combine sẽ nối các chuỗi thành một đường dẫn hợp lệ cho mọi hệ điều hành
            var uploadPath = Path.Combine (_webHostEnvironment.WebRootPath, subDirectory);

            // 2. Nếu thư mục chưa tồn tại, tạo mới
            if (!Directory.Exists (uploadPath))
            {
                Directory.CreateDirectory (uploadPath);
            }

            // 3. Tạo một tên file duy nhất để tránh trùng lặp
            // Sử dụng Guid để đảm bảo tên file là duy nhất
            var fileExtension = Path.GetExtension (file.FileName);
            var uniqueFileName = $"{Guid.NewGuid ( )}{fileExtension}";
            var filePath = Path.Combine (uploadPath, uniqueFileName);

            // 4. Lưu file vào server
            // Sử dụng 'using' để đảm bảo FileStream được giải phóng tài nguyên sau khi hoàn tất
            using (var stream = new FileStream (filePath, FileMode.Create))
            {
                // Copy nội dung của file được tải lên vào stream
                await file.CopyToAsync (stream);
            }

            // 5. Trả về đường dẫn tương đối của file để lưu vào database hoặc sử dụng sau này
            return Path.Combine (subDirectory, uniqueFileName).Replace ("\\", "/");
        }

        public async Task<(byte[]? fileContents, string? contentType, string? fileName)> DownloadFileAsync(string relativePath)
        {
            // 1. Lấy đường dẫn vật lý tuyệt đối của file
            var filePath = Path.Combine (_webHostEnvironment.WebRootPath, relativePath);

            // 2. Kiểm tra file có tồn tại không
            if (!File.Exists (filePath))
            {
                return (null, null, null);
            }

            // 3. Đọc nội dung file
            var fileContents = await File.ReadAllBytesAsync (filePath);
            var contentType = GetMimeType (filePath);
            var fileName = Path.GetFileName (filePath);

            return (fileContents, contentType, fileName);
        }

        public Task<bool> DeleteFileAsync(string relativePath)
        {
            // 1. Lấy đường dẫn vật lý tuyệt đối của file
            var filePath = Path.Combine (_webHostEnvironment.WebRootPath, relativePath);

            // 2. Thử xóa file
            try
            {
                if (File.Exists (filePath))
                {
                    File.Delete (filePath);
                    return Task.FromResult (true);
                }
                return Task.FromResult (false);
            }
            catch (Exception)
            {
                // Ghi log lỗi nếu cần
                return Task.FromResult (false);
            }
        }

        /// <summary>
        /// Helper method để lấy MIME type từ tên file
        /// </summary>
        private string GetMimeType(string fileName)
        {
            var provider = new Microsoft.AspNetCore.StaticFiles.FileExtensionContentTypeProvider ( );
            if (!provider.TryGetContentType (fileName, out var contentType))
            {
                contentType = "application/octet-stream"; // Default MIME type
            }
            return contentType;
        }

        public async Task<string> MoveFileAsync(string sourceRelativePath, string destSubDirectory)
        {
            var root = _webHostEnvironment.WebRootPath ?? Path.Combine (AppContext.BaseDirectory, "wwwroot");
            var srcFull = Path.GetFullPath (Path.Combine (root, sourceRelativePath ?? string.Empty));
            if (!File.Exists (srcFull)) throw new FileNotFoundException ("Source file not found", srcFull);

            var destDir = Path.GetFullPath (Path.Combine (root, destSubDirectory ?? string.Empty));
            if (!destDir.StartsWith (Path.GetFullPath (root), StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException ("Invalid destination");

            Directory.CreateDirectory (destDir);
            var fileName = Path.GetFileName (srcFull);
            var destFull = Path.Combine (destDir, fileName);

            // Nếu khác volume → Copy rồi Delete
            if (Path.GetPathRoot (srcFull) != Path.GetPathRoot (destFull))
            {
                File.Copy (srcFull, destFull, overwrite: true);
                File.Delete (srcFull);
            }
            else
            {
                File.Move (srcFull, destFull, overwrite: true);
            }

            return Path.Combine (destSubDirectory, fileName).Replace ("\\", "/");
        }

        public bool Exists(string relativePath)
        {
            var root = _webHostEnvironment.WebRootPath ?? Path.Combine (AppContext.BaseDirectory, "wwwroot");
            var full = Path.GetFullPath (Path.Combine (root, relativePath ?? string.Empty));
            return File.Exists (full);
        }

    }
}
