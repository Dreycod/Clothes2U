namespace Shared;

public class APIResponse<T>
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public T? Data { get; set; }
    
    public static APIResponse<T> SuccessResponse(T data)
    {
        return new APIResponse<T>
        {
            Success = true,
            Data = data
        };
    }
    
    public static APIResponse<T> ErrorResponse(string errorMessage)
    {
        return new APIResponse<T>
        {
            Success = false,
            ErrorMessage = errorMessage
        };
    }
}