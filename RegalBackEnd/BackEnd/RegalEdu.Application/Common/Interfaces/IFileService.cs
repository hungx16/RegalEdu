using Microsoft.AspNetCore.Http;

namespace RegalEdu.Application.Common.Interfaces
{
    /// <summary>
    /// Interface định nghĩa các hoạt động xử lý file.
    /// </summary>
    public interface IFileService
    {
        /// <summary>
        /// Tải một file lên server.
        /// </summary>
        /// <param name="file">File được gửi từ request.</param>
        /// <param name="subDirectory">Thư mục con trong wwwroot để lưu file.</param>
        /// <returns>Đường dẫn tương đối của file đã lưu.</returns>
        Task<string> UploadFileAsync(IFormFile file, string subDirectory);

        /// <summary>
        /// Tải một file từ server.
        /// </summary>
        /// <param name="filePath">Đường dẫn tương đối của file.</param>
        /// <returns>Một tuple chứa nội dung file (byte array), content type và tên file.</returns>
        Task<(byte[]? fileContents, string? contentType, string? fileName)> DownloadFileAsync(string filePath);

        /// <summary>
        /// Xóa một file khỏi server.
        /// </summary>
        /// <param name="filePath">Đường dẫn tương đối của file.</param>
        /// <returns>True nếu xóa thành công, ngược lại là false.</returns>
        Task<bool> DeleteFileAsync(string filePath);
        Task<string> MoveFileAsync(string sourceRelativePath, string destSubDirectory);
        bool Exists(string relativePath);
    }
}
