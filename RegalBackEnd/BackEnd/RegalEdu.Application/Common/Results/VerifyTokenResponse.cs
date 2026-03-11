

using RegalEdu.Domain.Models;

namespace RegalEdu.Application.Common.Results
{
    public class VerifyTokenResponse
    {
        public bool Succeeded { get; set; }
        public string? Message { get; set; }
        public ApplicationUserModel? User { get; set; }
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
    }

}
