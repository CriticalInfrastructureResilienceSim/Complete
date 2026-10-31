namespace Cirs.Web.Services;

// Wraps ILogger (backed by Serilog) so message wording and levels stay consistent everywhere.
// Never pass passwords, password hashes or cookies into these methods.
public class AppLogger : IAppLogger
{
    private readonly ILogger<AppLogger> _logger;

    public AppLogger(ILogger<AppLogger> logger)
    {
        _logger = logger;
    }

    public void Info(string message) => _logger.LogInformation("{Message}", message);

    public void Warning(string message) => _logger.LogWarning("{Message}", message);

    public void Error(Exception exception, string message) => _logger.LogError(exception, "{Message}", message);

    public void LoginAttempt(string user, bool success, string? ipAddress, string? reason = null)
    {
        if (success)
        {
            _logger.LogInformation("Login succeeded for {User} from {Ip}", user, ipAddress ?? "unknown");
        }
        else
        {
            _logger.LogWarning("Login failed for {User} from {Ip}. Reason: {Reason}", user, ipAddress ?? "unknown", reason ?? "invalid credentials");
        }
    }

    public void SignUp(string user, bool success)
    {
        if (success)
        {
            _logger.LogInformation("New account created for {User}", user);
        }
        else
        {
            _logger.LogWarning("Sign up failed for {User}", user);
        }
    }

    public void Logout(string user) => _logger.LogInformation("User {User} logged out", user);

    public void ValidationFailed(string form, IEnumerable<string> errors) =>
        _logger.LogWarning("{Form} validation failed: {Errors}", form, string.Join("; ", errors));

    public void CrudAction(string entity, string action, object? id, string user) =>
        _logger.LogInformation("{Entity} {Id} {Action} by {User}", entity, id, action, user);
}
