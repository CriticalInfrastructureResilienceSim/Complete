using Cirs.Web.Validators;
using Cirs.Web.ViewModels;

namespace Cirs.Tests;

public class UserValidatorTests
{
    private readonly UserValidator _validator = new();

    private static SignUpViewModel ValidSignUp() => new()
    {
        FullName = "Ali Khan",
        Email = "ali@example.com",
        Password = "Strong123",
        ConfirmPassword = "Strong123"
    };

    [Fact]
    public void SignUp_WithValidData_Passes()
    {
        var result = _validator.ValidateSignUp(ValidSignUp());

        Assert.True(result.IsValid);
    }

    [Fact]
    public void SignUp_WithEmptyName_ReturnsNameRequired()
    {
        var model = ValidSignUp();
        model.FullName = "  ";

        var result = _validator.ValidateSignUp(model);

        Assert.Contains(ValidationMessages.FullNameRequired, result.Errors[nameof(SignUpViewModel.FullName)]);
    }

    [Theory]
    [InlineData("")]
    [InlineData("not-an-email")]
    [InlineData("a@b")]
    public void SignUp_WithBadEmail_Fails(string email)
    {
        var model = ValidSignUp();
        model.Email = email;

        var result = _validator.ValidateSignUp(model);

        Assert.True(result.HasError(nameof(SignUpViewModel.Email)));
    }

    [Theory]
    [InlineData("Short1")]
    [InlineData("alllowercase1")]
    [InlineData("ALLUPPERCASE1")]
    [InlineData("NoDigitsHere")]
    public void SignUp_WithWeakPassword_Fails(string password)
    {
        var model = ValidSignUp();
        model.Password = password;
        model.ConfirmPassword = password;

        var result = _validator.ValidateSignUp(model);

        Assert.True(result.HasError(nameof(SignUpViewModel.Password)));
    }

    [Fact]
    public void SignUp_WithMismatchedConfirmation_Fails()
    {
        var model = ValidSignUp();
        model.ConfirmPassword = "Different123";

        var result = _validator.ValidateSignUp(model);

        Assert.Contains(ValidationMessages.ConfirmPasswordMismatch, result.Errors[nameof(SignUpViewModel.ConfirmPassword)]);
    }

    [Fact]
    public void Login_WithEmptyFields_ReturnsBothErrors()
    {
        var result = _validator.ValidateLogin(new LoginViewModel());

        Assert.True(result.HasError(nameof(LoginViewModel.Email)));
        Assert.True(result.HasError(nameof(LoginViewModel.Password)));
    }

    [Fact]
    public void Login_WithValidFields_Passes()
    {
        var result = _validator.ValidateLogin(new LoginViewModel { Email = "ali@example.com", Password = "anything" });

        Assert.True(result.IsValid);
    }
}
