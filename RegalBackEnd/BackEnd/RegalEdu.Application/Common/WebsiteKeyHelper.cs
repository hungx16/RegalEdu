
using Newtonsoft.Json;
using RegalEdu.Domain.Models.DTO;
using System.Text;

namespace RegalEdu.Application.Common
{

    public static class WebsiteKeyHelper
    {
        public static async Task<List<WebsiteKey>> LoadWebsiteKeysAsync( )
        {
            var filePath = Path.Combine (AppContext.BaseDirectory, "Resources", "WebsiteKeys.json");
            if (!File.Exists (filePath)) return new List<WebsiteKey> ( );

            using var stream = File.OpenRead (filePath);
            var provinces = await System.Text.Json.JsonSerializer.DeserializeAsync<List<WebsiteKey>> (stream)
                ?? new List<WebsiteKey> ( );
            return provinces;
        }
        public static async Task SaveWebsiteKeysAsync(List<WebsiteKey> websiteKeys)
        {
            var filePath = Path.Combine (AppContext.BaseDirectory, "Resources", "WebsiteKeys.json");

            // 1) Serialize ra chuỗi JSON
            string jsonData = JsonConvert.SerializeObject (
                websiteKeys ?? new List<WebsiteKey> ( ),
                Formatting.Indented // để dễ đọc
            );

            // 2) Đảm bảo thư mục tồn tại
            var dir = Path.GetDirectoryName (filePath);
            if (!Directory.Exists (dir))
                Directory.CreateDirectory (dir!);

            // 3) Ghi file (UTF8 không BOM)
            await File.WriteAllTextAsync (
                filePath,
                jsonData,
                new UTF8Encoding (encoderShouldEmitUTF8Identifier: false)
            );
        }
        public static async Task<List<WebsiteKey>> LoadEnWebsiteKeysAsync( )
        {
            var filePath = Path.Combine (AppContext.BaseDirectory, "Resources", "EnWebsiteKeys.json");
            if (!File.Exists (filePath)) return new List<WebsiteKey> ( );

            using var stream = File.OpenRead (filePath);
            var provinces = await System.Text.Json.JsonSerializer.DeserializeAsync<List<WebsiteKey>> (stream)
                ?? new List<WebsiteKey> ( );
            return provinces;
        }
        public static async Task SaveEnWebsiteKeysAsync(List<WebsiteKey> websiteKeys)
        {
            var filePath = Path.Combine (AppContext.BaseDirectory, "Resources", "EnWebsiteKeys.json");

            // 1) Serialize ra chuỗi JSON
            string jsonData = JsonConvert.SerializeObject (
                websiteKeys ?? new List<WebsiteKey> ( ),
                Formatting.Indented // để dễ đọc
            );

            // 2) Đảm bảo thư mục tồn tại
            var dir = Path.GetDirectoryName (filePath);
            if (!Directory.Exists (dir))
                Directory.CreateDirectory (dir!);

            // 3) Ghi file (UTF8 không BOM)
            await File.WriteAllTextAsync (
                filePath,
                jsonData,
                new UTF8Encoding (encoderShouldEmitUTF8Identifier: false)
            );
        }
    }
}
