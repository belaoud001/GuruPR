using Microsoft.AspNetCore.Mvc;

using GuruPR.Application.Exceptions.Account;

namespace GuruPR.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly RequestDelegate _next;
    private readonly bool _isDevelopment;

    public ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger, RequestDelegate next, IHostEnvironment hostEnvironment)
    {
        _logger = logger;
        _next = next;
        _isDevelopment = hostEnvironment.IsDevelopment();
    }

    public async Task Invoke(HttpContext httpContext)
    {
        try
        {
            await _next.Invoke(httpContext);
        }
        catch (Exception exception)
        {
            await HandleExceptionAsync(httpContext, exception);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        if (context.Response.HasStarted)
        {
            _logger.LogWarning("Response has already started, cannot handle exception");

            return;
        }

        var (statusCode, title) = exception switch
        {
            // Client Errors
            RegistrationFailedException => (StatusCodes.Status400BadRequest,   "User registration failed"),
            RefreshTokenException       => (StatusCodes.Status401Unauthorized, "Invalid Refresh Token"),
            LoginFailedException        => (StatusCodes.Status401Unauthorized, "Login Failed"),
            UserAlreadyExistsException  => (StatusCodes.Status409Conflict,     "User Already Exists"),

            // Server Errors
            _ => (StatusCodes.Status500InternalServerError, "Internal Server Error")
        };

        var isServerError = statusCode >= 500;
        var showDetails = _isDevelopment || !isServerError;

        var problem = new ProblemDetails
        {
            Status   = statusCode,
            Title    = title,
            Detail   = showDetails ? exception.Message : "An unexpected error occurred.",
            Instance = context.Request.Path
        };

        problem.Extensions["traceId"] = context.TraceIdentifier;

        if (exception is IValidationException validationException && validationException.Errors.Count > 0)
        {
            problem.Extensions["errors"] = validationException.Errors;
        }

        _logger.Log(isServerError ? LogLevel.Error : LogLevel.Warning,
                    exception,
                    "Handled {ExceptionType}: {Title} (HTTP {StatusCode})",
                    exception.GetType().Name,
                    title,
                    statusCode);

        context.Response.Clear();
        context.Response.StatusCode  = statusCode;
        context.Response.ContentType = "application/problem+json";

        await context.Response.WriteAsJsonAsync(problem);
    }
}
