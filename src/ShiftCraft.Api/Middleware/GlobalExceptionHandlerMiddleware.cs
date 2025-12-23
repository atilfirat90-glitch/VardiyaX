using System.Net;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace ShiftCraft.Api.Middleware;

public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

    public GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var response = context.Response;
        response.ContentType = "application/json";

        var errorResponse = new ErrorResponse
        {
            TraceId = context.TraceIdentifier,
            Timestamp = DateTime.UtcNow
        };

        switch (exception)
        {
            case DbUpdateException dbEx:
                response.StatusCode = (int)HttpStatusCode.Conflict;
                errorResponse.Message = "Veritabanı işlemi başarısız oldu.";
                errorResponse.Detail = dbEx.InnerException?.Message ?? dbEx.Message;
                _logger.LogError(dbEx, "Database error occurred. TraceId: {TraceId}", errorResponse.TraceId);
                break;

            case ArgumentException argEx:
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResponse.Message = "Geçersiz parametre.";
                errorResponse.Detail = argEx.Message;
                _logger.LogWarning(argEx, "Argument error. TraceId: {TraceId}", errorResponse.TraceId);
                break;

            case KeyNotFoundException notFoundEx:
                response.StatusCode = (int)HttpStatusCode.NotFound;
                errorResponse.Message = "Kayıt bulunamadı.";
                errorResponse.Detail = notFoundEx.Message;
                _logger.LogWarning(notFoundEx, "Resource not found. TraceId: {TraceId}", errorResponse.TraceId);
                break;

            case InvalidOperationException invalidOpEx:
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResponse.Message = "Geçersiz işlem.";
                errorResponse.Detail = invalidOpEx.Message;
                _logger.LogWarning(invalidOpEx, "Invalid operation. TraceId: {TraceId}", errorResponse.TraceId);
                break;

            case OperationCanceledException:
                response.StatusCode = (int)HttpStatusCode.RequestTimeout;
                errorResponse.Message = "İşlem iptal edildi.";
                _logger.LogInformation("Operation cancelled. TraceId: {TraceId}", errorResponse.TraceId);
                break;

            default:
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                errorResponse.Message = "Beklenmeyen bir hata oluştu.";
                _logger.LogError(exception, "Unhandled exception. TraceId: {TraceId}", errorResponse.TraceId);
                break;
        }

        var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        await response.WriteAsync(JsonSerializer.Serialize(errorResponse, jsonOptions));
    }
}

public class ErrorResponse
{
    public string Message { get; set; } = string.Empty;
    public string? Detail { get; set; }
    public string TraceId { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}
