namespace Shared.DTO
{
    /// <summary>
    /// DTO générique pour encapsuler les résultats des opérations de service
    /// </summary>
    /// <typeparam name="T">Type de données retournées</typeparam>
    public class ServiceResult<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public string? ErrorMessage { get; set; }
        public int? StatusCode { get; set; }

        public static ServiceResult<T> SuccessResult(T data)
        {
            return new ServiceResult<T>
            {
                Success = true,
                Data = data,
                ErrorMessage = null,
                StatusCode = 200
            };
        }

        public static ServiceResult<T> FailureResult(string errorMessage, int statusCode = 400)
        {
            return new ServiceResult<T>
            {
                Success = false,
                Data = default,
                ErrorMessage = errorMessage,
                StatusCode = statusCode
            };
        }
    }

    /// <summary>
    /// Version sans données (pour les opérations void)
    /// </summary>
    public class ServiceResult
    {
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public int? StatusCode { get; set; }

        public static ServiceResult SuccessResult()
        {
            return new ServiceResult
            {
                Success = true,
                ErrorMessage = null,
                StatusCode = 200
            };
        }

        public static ServiceResult FailureResult(string errorMessage, int statusCode = 400)
        {
            return new ServiceResult
            {
                Success = false,
                ErrorMessage = errorMessage,
                StatusCode = statusCode
            };
        }
    }
}