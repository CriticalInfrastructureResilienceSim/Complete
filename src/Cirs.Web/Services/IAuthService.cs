using Cirs.Web.Models;
using Cirs.Web.Validators;
using Cirs.Web.ViewModels;

namespace Cirs.Web.Services;

public record SignUpResult(bool Success, FormValidationResult Validation, User? User);

public record LoginResult(bool Success, FormValidationResult Validation, User? User);

public interface IAuthService
{
    Task<SignUpResult> SignUpAsync(SignUpViewModel model);

    Task<LoginResult> LoginAsync(LoginViewModel model, string? ipAddress);
}
