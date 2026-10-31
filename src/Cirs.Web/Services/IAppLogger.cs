namespace Cirs.Web.Services;

// The only logging entry point for application code. Nothing outside AppLogger touches Serilog.
public interface IAppLogger
{
    void Info(string message);
    void Warning(string message);
    void Error(Exception exception, string message);
    void LoginAttempt(string user, bool success, string? ipAddress, string? reason = null);
    void SignUp(string user, bool success);
    void Logout(string user);
    void ValidationFailed(string form, IEnumerable<string> errors);
    void CrudAction(string entity, string action, object? id, string user);
}
