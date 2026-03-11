using Microsoft.AspNetCore.Mvc;

namespace RegalEdu.Application.Common.Results
{
    public static class ResultExtensions
    {
        public static IActionResult ToApiResponse(this Result result)
        {
            if (result.Succeeded)
                return new OkObjectResult (ApiResponse<string>.Success (result.Data?.ToString ( ) ?? "Success"));
            return new BadRequestObjectResult (ApiResponse<string>.Failure (result.Errors));
        }

        public static IActionResult ToApiResponse<T>(this Result<T> result)
        {
            if (result.Succeeded)
                return new OkObjectResult (ApiResponse<T>.Success (result.Data!));
            return new BadRequestObjectResult (ApiResponse<T>.Failure (result.Errors));
        }
    }
}
