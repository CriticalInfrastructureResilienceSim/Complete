namespace Cirs.Web.Validators;

// Every help text lives here. Help stays visible under the input even when an error appears.
public static class FieldHelp
{
    public const string FullName = "2 to 100 characters. Letters, spaces, dots, apostrophes and hyphens.";
    public const string Email = "This is the address you will use to log in.";
    public const string SignUpPassword = "8 to 64 characters with an uppercase letter, a lowercase letter and a number.";
    public const string ConfirmPassword = "Type the same password again.";
    public const string LoginPassword = "Enter the password you chose at sign up.";
}
