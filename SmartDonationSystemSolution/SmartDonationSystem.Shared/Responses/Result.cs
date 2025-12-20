using System.Net;

namespace SmartDonationSystem.Shared.Responses
{
    public class Result<T>
    {
        public HttpStatusCode statusCode { get; set; }
        public bool Success { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }
        //Core builder
        public static Result<T> Create(bool success, HttpStatusCode statusCode, string? message, T? data = default)
        {
            return new Result<T>
            {
                Success = success,
                statusCode = statusCode,
                Message = message ?? SetDefaultMessage(statusCode),
                Data = data
            };
        }
        private static string SetDefaultMessage(HttpStatusCode statusCode)
        {
            return statusCode switch
            {
                HttpStatusCode.OK => "Success",
                HttpStatusCode.Created => "Created successfully",
                HttpStatusCode.NoContent => "No content",
                HttpStatusCode.BadRequest => "Bad Request",
                HttpStatusCode.Unauthorized => "Unauthorized",
                HttpStatusCode.Forbidden => "Forbidden",
                HttpStatusCode.NotFound => "Not Found",
                HttpStatusCode.InternalServerError => "Internal Server Error",
                _ => "An error occurred"
            };
        }

        // Factory methods - Success

        /// <summary>
        /// Returns a successful result with data.
        /// </summary>
        /// <param name="data">The returned data.</param>
        /// <param name="message">Optional success message.</param>
        public static Result<T> Ok(T data, string? message = null) => Create(true, HttpStatusCode.OK, message, data);
        /// <summary>
        /// Returns a successful result indicating a new resource was created.
        /// </summary>
        /// <param name="data">The created resource.</param>
        /// <param name="message">Optional message.</param>
        public static Result<T> Created(T data, string? message = null) => Create(true, HttpStatusCode.Created, message, data);
        /// <summary>
        /// Returns a successful result with no content.
        /// </summary>
        /// <param name="message">Optional message.</param>
        public static Result<T> NoContent(string? message = null) => Create(true, HttpStatusCode.NoContent, message);

        //Factory methods - Failure

        /// <summary>
        /// Returns a failure result for a bad request.
        /// </summary>
        public static Result<T> BadRequest(string? message = null) => Create(false, HttpStatusCode.BadRequest, message);
        /// <summary>
        /// Returns a failure result for unauthorized access.
        /// </summary>
        /// <param name="message">Optional error message.</param>
        public static Result<T> Unauthorized(string? message = null) => Create(false, HttpStatusCode.Unauthorized, message);
        /// <summary>
        /// Returns a failure result for forbidden access.
        /// </summary>
        /// <param name="message">Optional error message.</param>
        public static Result<T> Forbidden(string? message = null) => Create(false, HttpStatusCode.Forbidden, message);
        /// <summary>
        /// Returns a failure result when a resource is not found.
        /// </summary>
        public static Result<T> NotFound(string? message = null) => Create(false, HttpStatusCode.NotFound, message);
        /// <summary>
        /// Returns a failure result for internal server errors.
        /// </summary>
        /// <param name="message">Optional error message.</param>
        public static Result<T> ServerError(string? message = null) => Create(false, HttpStatusCode.InternalServerError, message);
    }
}
