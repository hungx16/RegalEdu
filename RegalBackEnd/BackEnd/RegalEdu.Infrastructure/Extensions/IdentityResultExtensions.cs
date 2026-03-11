using RegalEdu.Application.Common.Results;

namespace RegalEdu.Infrastructure.Extensions
{
    public static class IdentityResultExtensions
    {
        public static Result ToApplicationResult(this Microsoft.AspNetCore.Identity.IdentityResult result)
        {
            return result.Succeeded
                ? Result.Success()
                : Result.Failure(string.Join("<br/>", result.Errors.Select(e => $"{e.Code}: {e.Description}")));
        }
    }
}
