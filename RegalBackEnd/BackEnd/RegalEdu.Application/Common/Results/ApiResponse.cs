namespace RegalEdu.Application.Common.Results
{
    public class ApiResponse<T>
    {
        public int StatusCode { get; set; }
        public bool Succeeded { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }
        public List<string>? Errors { get; set; }

        public static ApiResponse<T> Success(T data, string? message = null)
            => new() { StatusCode = 200, Succeeded = true, Data = data, Message = message };

        public static ApiResponse<T> Failure(string message, List<string>? errors = null, int statusCode = 400)
            => new() { StatusCode = statusCode, Succeeded = false, Message = message, Errors = errors };
    }
}
