using RegalEdu.Domain.Models.DTO;


namespace RegalEdu.Application.Common.Request
{
    public class DeleteSupportingDocumentRequest
    {
        public List<string>? Ids { get; set; } = new List<string> ( );
        public List<WebsiteKey>? WebsiteKeys { get; set; } = new List<WebsiteKey> ( );
    }
}
