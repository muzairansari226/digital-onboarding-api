namespace CustomerPortal.Application.Helper
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }

        public string Message { get; set; } = string.Empty;

        public T? Data { get; set; }

        public List<string> Errors { get; set; } = new();

        public int StatusCode { get; set; }

        public static ApiResponse<T> SuccessResponse(
            T data, string message = "Operation successful", int statusCode = 200)
            => new() { Success = true, Message = message, Data = data, StatusCode = statusCode };

        public static ApiResponse<T> ErrorResponse(
            string message, int statusCode, List<string>? errors = null)
            => new()
            {
                Success = false,
                Message = message,
                Data = default,
                Errors = errors ?? new List<string>(),
                StatusCode = statusCode
            };

        public static ApiResponse<T> ErrorResponse(
            string message, int statusCode, T? data, List<string>? errors = null)
            => new()
            {
                Success = false,
                Message = message,
                Data = data,
                Errors = errors ?? new List<string>(),
                StatusCode = statusCode
            };
    }
}
