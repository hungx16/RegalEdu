
using System.Text.Json;

namespace RegalEdu.Application.Common
{
    public class DocumentType
    {
        public required int DocumentTypeId { get; set; }
        public required string DocumentTypeName { get; set; }
    }
    public static class DocumentTypeFileHelper
    {
        public static async Task<List<DocumentType>> LoadDocumentTypesAsync( )
        {
            var filePath = Path.Combine (AppContext.BaseDirectory, "Resources", "DocumentTypes.json");
            if (!File.Exists (filePath)) return new List<DocumentType> ( );

            using var stream = File.OpenRead (filePath);
            var DocumentTypes = await JsonSerializer.DeserializeAsync<List<DocumentType>> (stream)
                ?? new List<DocumentType> ( );
            return DocumentTypes;
        }
    }
}
