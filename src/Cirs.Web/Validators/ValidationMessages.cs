namespace Cirs.Web.Validators;

// Every error sentence lives here so the same problem always uses the same wording on every screen.
public static class ValidationMessages
{
    public const string FullNameRequired = "Full name is required.";
    public const string FullNameLength = "Full name must be 2 to 100 characters.";
    public const string FullNameCharacters = "Full name can contain only letters, spaces, dots, apostrophes and hyphens.";

    public const string EmailRequired = "Email is required.";
    public const string EmailInvalid = "Enter a valid email address, for example name@example.com.";
    public const string EmailTooLong = "Email must be 150 characters or fewer.";
    public const string EmailTaken = "An account with this email already exists.";

    public const string PasswordRequired = "Password is required.";
    public const string PasswordLength = "Password must be 8 to 64 characters.";
    public const string PasswordComplexity = "Password must include an uppercase letter, a lowercase letter and a number.";
    public const string ConfirmPasswordRequired = "Please confirm your password.";
    public const string ConfirmPasswordMismatch = "Passwords do not match.";

    public const string InvalidCredentials = "Invalid email or password.";

    public static string AccountLocked(int minutesLeft) =>
        $"Too many failed attempts. This account is locked for about {minutesLeft} more minute(s).";
}
