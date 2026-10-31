using System.Text.RegularExpressions;
using Cirs.Web.ViewModels;

namespace Cirs.Web.Validators;

// All rules for the sign up and login forms. Controllers never contain these rules.
public class UserValidator
{
    public const int FullNameMin = 2;
    public const int FullNameMax = 100;
    public const int EmailMax = 150;
    public const int PasswordMin = 8;
    public const int PasswordMax = 64;

    private static readonly Regex EmailPattern =
        new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

    private static readonly Regex NamePattern =
        new(@"^\p{L}[\p{L} .'\-]*$", RegexOptions.Compiled);

    public FormValidationResult ValidateSignUp(SignUpViewModel model)
    {
        var result = new FormValidationResult();

        ValidateFullName(model.FullName, result);
        ValidateEmail(model.Email, result);

        if (string.IsNullOrEmpty(model.Password))
        {
            result.Add(nameof(SignUpViewModel.Password), ValidationMessages.PasswordRequired);
        }
        else
        {
            if (model.Password.Length < PasswordMin || model.Password.Length > PasswordMax)
            {
                result.Add(nameof(SignUpViewModel.Password), ValidationMessages.PasswordLength);
            }

            if (!model.Password.Any(char.IsUpper) || !model.Password.Any(char.IsLower) || !model.Password.Any(char.IsDigit))
            {
                result.Add(nameof(SignUpViewModel.Password), ValidationMessages.PasswordComplexity);
            }
        }

        if (string.IsNullOrEmpty(model.ConfirmPassword))
        {
            result.Add(nameof(SignUpViewModel.ConfirmPassword), ValidationMessages.ConfirmPasswordRequired);
        }
        else if (model.ConfirmPassword != model.Password)
        {
            result.Add(nameof(SignUpViewModel.ConfirmPassword), ValidationMessages.ConfirmPasswordMismatch);
        }

        return result;
    }

    public FormValidationResult ValidateLogin(LoginViewModel model)
    {
        var result = new FormValidationResult();

        ValidateEmail(model.Email, result);

        if (string.IsNullOrEmpty(model.Password))
        {
            result.Add(nameof(LoginViewModel.Password), ValidationMessages.PasswordRequired);
        }

        return result;
    }

    private static void ValidateFullName(string? fullName, FormValidationResult result)
    {
        var name = (fullName ?? string.Empty).Trim();
        var field = nameof(SignUpViewModel.FullName);

        if (name.Length == 0)
        {
            result.Add(field, ValidationMessages.FullNameRequired);
            return;
        }

        if (name.Length < FullNameMin || name.Length > FullNameMax)
        {
            result.Add(field, ValidationMessages.FullNameLength);
        }

        if (!NamePattern.IsMatch(name))
        {
            result.Add(field, ValidationMessages.FullNameCharacters);
        }
    }

    private static void ValidateEmail(string? email, FormValidationResult result)
    {
        var value = (email ?? string.Empty).Trim();
        var field = nameof(SignUpViewModel.Email);

        if (value.Length == 0)
        {
            result.Add(field, ValidationMessages.EmailRequired);
            return;
        }

        if (value.Length > EmailMax)
        {
            result.Add(field, ValidationMessages.EmailTooLong);
        }
        else if (!EmailPattern.IsMatch(value))
        {
            result.Add(field, ValidationMessages.EmailInvalid);
        }
    }
}
