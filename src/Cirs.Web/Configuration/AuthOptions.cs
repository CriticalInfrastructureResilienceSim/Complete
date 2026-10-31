namespace Cirs.Web.Configuration;

// Bound from the "Auth" section of appsettings.json so limits can change without recompiling.
public class AuthOptions
{
    public const string SectionName = "Auth";

    public int MaxFailedAttempts { get; set; } = 5;
    public int LockoutMinutes { get; set; } = 5;
    public int CookieExpireMinutes { get; set; } = 30;
    public int SessionIdleMinutes { get; set; } = 30;
    public int RememberMeDays { get; set; } = 7;
    public int BcryptWorkFactor { get; set; } = 11;
}
