namespace ProductsMicroService.API.Wrappers
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }
        public IEnumerable<string>? Errors { get; set; }

        public ApiResponse(T data, string? message = null)
        {
            Success = true;
            Data = data;
            Message = message;
        }

        public ApiResponse(string message, IEnumerable<string>? errors = null)
        {
            Success = false;
            Message = message;
            Errors = errors;
        }
    }
}
