using System.Security.Claims;
using Cirs.Web.Configuration;
using Cirs.Web.Extensions;
using Cirs.Web.Services;
using Cirs.Web.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Cirs.Web.Controllers;

public class AccountController : Controller
{
    private readonly IAuthService _authService;
    private readonly IAppLogger _logger;
    private readonly AuthOptions _authOptions;

    public AccountController(IAuthService authService, IAppLogger logger, IOptions<AuthOptions> authOptions)
    {
        _authService = authService;
        _logger = logger;
        _authOptions = authOptions.Value;
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult SignUp()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Home");
        }

        return View(new SignUpViewModel());
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SignUp(SignUpViewModel model)
    {
        var result = await _authService.SignUpAsync(model);

        if (!result.Success)
        {
            ModelState.AddValidationResult(result.Validation);
            model.Password = string.Empty;
            model.ConfirmPassword = string.Empty;
            return View(model);
        }

        TempData["Success"] = "Your account was created. You can now log in.";
        return RedirectToAction(nameof(Login));
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Home");
        }

        ViewData["ReturnUrl"] = returnUrl;
        return View(new LoginViewModel());
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
        var result = await _authService.LoginAsync(model, ipAddress);

        if (!result.Success || result.User is null)
        {
            ModelState.AddValidationResult(result.Validation);
            model.Password = string.Empty;
            ViewData["ReturnUrl"] = returnUrl;
            return View(model);
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, result.User.Id.ToString()),
            new(ClaimTypes.Name, result.User.FullName),
            new(ClaimTypes.Email, result.User.Email),
            new(ClaimTypes.Role, result.User.Role)
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var properties = new AuthenticationProperties
        {
            IsPersistent = model.RememberMe,
            ExpiresUtc = model.RememberMe ? DateTimeOffset.UtcNow.AddDays(_authOptions.RememberMeDays) : null
        };

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity),
            properties);

        // The cookie proves who the user is. Session only keeps small per-visit state.
        HttpContext.Session.SetString("LoginTimeUtc", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss 'UTC'"));

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return LocalRedirect(returnUrl);
        }

        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        var email = User.FindFirstValue(ClaimTypes.Email) ?? "unknown";

        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        HttpContext.Session.Clear();
        _logger.Logout(email);

        TempData["Success"] = "You have been logged out.";
        return RedirectToAction(nameof(Login));
    }

    [HttpGet]
    [Authorize]
    public IActionResult Profile()
    {
        var visits = (HttpContext.Session.GetInt32("ProfileVisits") ?? 0) + 1;
        HttpContext.Session.SetInt32("ProfileVisits", visits);

        var model = new ProfileViewModel
        {
            FullName = User.Identity?.Name ?? string.Empty,
            Email = User.FindFirstValue(ClaimTypes.Email) ?? string.Empty,
            Role = User.FindFirstValue(ClaimTypes.Role) ?? string.Empty,
            SessionLoginTime = HttpContext.Session.GetString("LoginTimeUtc") ?? "not set (log in again)",
            ProfileVisits = visits
        };

        return View(model);
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult AccessDenied() => View();
}
