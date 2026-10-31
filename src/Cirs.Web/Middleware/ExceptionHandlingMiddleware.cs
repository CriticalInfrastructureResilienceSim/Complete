using Cirs.Web.Services;

namespace Cirs.Web.Middleware;

// Catches anything a controller throws. The user gets a friendly page with a short reference id;
// the full exception goes to the log under that same id.
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IAppLogger logger)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            var errorId = Guid.NewGuid().ToString("N")[..8];
            logger.Error(ex, $"Unhandled exception. ErrorId={errorId} {context.Request.Method} {context.Request.Path}");

            // If the response already started we cannot redirect, so let the server close the connection.
            if (context.Response.HasStarted)
            {
                throw;
            }

            context.Response.Redirect($"/Home/Error?errorId={errorId}");
        }
    }
}
