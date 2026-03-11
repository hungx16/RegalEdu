namespace RegalEdu.Application.Common
{
    public class Ward
    {
        public required string WardCode { get; set; }
        public required string WardName { get; set; }
        public required string ProvinceCode { get; set; }
        public string? Level { get; set; }
    }

    public static class WardFileHelper
    {
        public static async Task<List<Ward>> LoadWardsAsync( )
        {
            var filePath = Path.Combine (AppContext.BaseDirectory, "Resources", "wards.json");
            if (!File.Exists (filePath)) return new List<Ward> ( );

            using var stream = File.OpenRead (filePath);
            var wards = await System.Text.Json.JsonSerializer.DeserializeAsync<List<Ward>> (stream)
                ?? new List<Ward> ( );
            return wards;
        }
    }
}
