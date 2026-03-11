
using System.Text.Json;

namespace RegalEdu.Application.Common
{
    public class Province
    {
        public required string ProvinceCode { get; set; }
        public required string ProvinceName { get; set; }

        public required string EnProvinceName { get; set; }
    }
    public static class ProvinceFileHelper
    {
        public static async Task<List<Province>> LoadProvincesAsync( )
        {
            var filePath = Path.Combine (AppContext.BaseDirectory, "Resources", "provinces.json");
            if (!File.Exists (filePath)) return new List<Province> ( );

            using var stream = File.OpenRead (filePath);
            var provinces = await JsonSerializer.DeserializeAsync<List<Province>> (stream)
                ?? new List<Province> ( );
            return provinces;
        }
    }
}
