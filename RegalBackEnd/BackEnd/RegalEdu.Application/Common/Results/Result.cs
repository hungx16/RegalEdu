using Microsoft.AspNetCore.Mvc;
using RegalEdu.Shared;

namespace RegalEdu.Application.Common.Results
{
    public class Result
    {
        public bool Succeeded { get; set; }
        public string Errors { get; set; } = string.Empty;
        public object? Data { get; set; }

        public Result( )
        {

        }
        internal Result(bool succeeded, string error)
        {
            Succeeded = succeeded;
            Errors = error;
        }

        public static Result Success( )
        {
            return new Result (true, string.Empty);
        }

        public static Result Failure(string error)
        {
            return new Result (false, error);
        }
        public bool GetStatus( )
        {
            return Succeeded;
        }
        public string GetError( )
        {
            return Errors;
        }
        public static implicit operator ActionResult(Result result)
        {
            return (ActionResult)result.ToApiResponse ( );
        }
        public static Result Success(object data)
        {
            return new Result
            {
                Succeeded = true,
                Errors = string.Empty,
                Data = data
            };
        }

    }

    public class ResetPasswordResult
    {
        public string Email { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class Result<T>
    {
        public bool Succeeded { get; set; }
        public string Errors { get; set; } = string.Empty;
        public T? Data { get; set; }
        public static implicit operator ActionResult(Result<T> result)
        {
            return (ActionResult)result.ToApiResponse ( );
        }

        public static Result<T> Success(T data)
        {
            return new Result<T> { Succeeded = true, Data = data };
        }

        public static Result<T> Failure(string error, Exception? ex = null)
        {
            var message = error;
            if (ex != null)
            {
                // Ghép lỗi, ưu tiên show message dev/debug, không show cho end-user ở production
                message += Functions.GetFullExceptionMessage (ex);
            }
            return new Result<T> { Succeeded = false, Errors = message };
        }
        public static Result<T> Failure(Exception? ex = null)
        {
            var message = "";
            if (ex != null)
            {
                // Ghép lỗi, ưu tiên show message dev/debug, không show cho end-user ở production
                message += $" | Exception: {ex.Message}";
                if (ex.InnerException != null)
                {
                    message += $" | Inner: {ex.InnerException.Message}";
                }
            }
            return new Result<T> { Succeeded = false, Errors = message };
        }
    }
}
