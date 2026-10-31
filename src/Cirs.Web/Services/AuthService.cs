using Cirs.Web.Configuration;
using Cirs.Web.Data;
using Cirs.Web.Models;
using Cirs.Web.Validators;
using Cirs.Web.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Cirs.Web.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _db;
    private readonly UserValidator _validator;
    private readonly IAppLogger _logger;
    private readonly AuthOptions _options;

    public AuthService(AppDbContext db, UserValidator validator, IAppLogger logger, IOptions<AuthOptions> options)
    {
        _db = db;
        _validator = validator;
        _logger = logger;
        _options = options.Value;
    }

    public async Task<SignUpResult> SignUpAsync(SignUpViewModel model)
    {
        var email = NormalizeEmail(model.Email);
        var validation = _validator.ValidateSignUp(model);

        // Only hit the database when the email itself looks valid.
        if (!validation.HasError(nameof(SignUpViewModel.Email)) &&
            await _db.Users.AnyAsync(u => u.Email == email))
        {
            validation.Add(nameof(SignUpViewModel.Email), ValidationMessages.EmailTaken);
        }

        if (!validation.IsValid)
        {
            _logger.ValidationFailed("SignUpForm", validation.AllMessages());
            _logger.SignUp(email, false);
            return new SignUpResult(false, validation, null);
        }

        var user = new User
        {
            FullName = model.FullName.Trim(),
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password, _options.BcryptWorkFactor),
            Role = UserRoles.User,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _db.Users.Add(user);

        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            // Two people signing up with the same email at the same moment are stopped by the unique index.
            _db.Entry(user).State = EntityState.Detached;

            if (await _db.Users.AsNoTracking().AnyAsync(u => u.Email == email))
            {
                _logger.Warning($"Duplicate sign up blocked by unique index for {email}: {ex.GetType().Name}");
                validation.Add(nameof(SignUpViewModel.Email), ValidationMessages.EmailTaken);
                _logger.SignUp(email, false);
                return new SignUpResult(false, validation, null);
            }

            throw;
        }

        _logger.SignUp(email, true);
        return new SignUpResult(true, validation, user);
    }

    public async Task<LoginResult> LoginAsync(LoginViewModel model, string? ipAddress)
    {
        var email = NormalizeEmail(model.Email);
        var validation = _validator.ValidateLogin(model);

        if (!validation.IsValid)
        {
            _logger.ValidationFailed("LoginForm", validation.AllMessages());
            return new LoginResult(false, validation, null);
        }

        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);

        // The same message is shown whether the email or the password was wrong, so nobody can probe for accounts.
        if (user is null || !user.IsActive)
        {
            _logger.LoginAttempt(email, false, ipAddress, user is null ? "unknown email" : "account disabled");
            validation.Add(string.Empty, ValidationMessages.InvalidCredentials);
            return new LoginResult(false, validation, null);
        }

        var now = DateTime.UtcNow;

        if (user.LockoutEnd is not null && user.LockoutEnd > now)
        {
            var minutesLeft = (int)Math.Ceiling((user.LockoutEnd.Value - now).TotalMinutes);
            _logger.LoginAttempt(email, false, ipAddress, "account locked");
            validation.Add(string.Empty, ValidationMessages.AccountLocked(minutesLeft));
            return new LoginResult(false, validation, null);
        }

        if (!BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
        {
            user.FailedLoginCount++;

            if (user.FailedLoginCount >= _options.MaxFailedAttempts)
            {
                user.LockoutEnd = now.AddMinutes(_options.LockoutMinutes);
                user.FailedLoginCount = 0;
                _logger.Warning($"Account {email} locked for {_options.LockoutMinutes} minute(s) after repeated failed logins from {ipAddress ?? "unknown"}");
            }

            await _db.SaveChangesAsync();
            _logger.LoginAttempt(email, false, ipAddress, "wrong password");
            validation.Add(string.Empty, ValidationMessages.InvalidCredentials);
            return new LoginResult(false, validation, null);
        }

        user.FailedLoginCount = 0;
        user.LockoutEnd = null;
        user.LastLoginAt = now;
        await _db.SaveChangesAsync();

        _logger.LoginAttempt(email, true, ipAddress);
        return new LoginResult(true, validation, user);
    }

    private static string NormalizeEmail(string? email) => (email ?? string.Empty).Trim().ToLowerInvariant();
}
