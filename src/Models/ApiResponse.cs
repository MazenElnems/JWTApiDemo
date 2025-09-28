namespace JWTAuthApp.Models
{
    public class ApiResponse
    {
        public IEnumerable<string> Errors { get; set; }
        public bool IsSuccess { get; set; }
        public string Message { get; set; }

        public static ApiResponse Success(string message = "")
        {
            return new ApiResponse
            {
                IsSuccess = true,
                Message = message,
                Errors = Array.Empty<string>()
            };
        }

        public static ApiResponse Failure(IEnumerable<string> errors, string message = "")
        {
            return new ApiResponse
            {
                IsSuccess = false,
                Message = message,
                Errors = errors
            };
        }
    }

    public class ApiResponse<T> : ApiResponse
    {
        public T Data { get; set; }

        public static ApiResponse<T> Success(T data, string message = "")
        {
            return new ApiResponse<T>
            {
                Data = data,
                IsSuccess = true,
                Message = message,
                Errors = Array.Empty<string>()
            };
        }
    }
}
