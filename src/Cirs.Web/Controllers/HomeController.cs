using Cirs.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cirs.Web.Controllers;

public class HomeController : Controller
{
    private readonly IWebHostEnvironment _environment;

    public HomeController(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    [AllowAnonymous]
    public IActionResult Index() => View();

    [AllowAnonymous]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error(string? errorId, int? code)
    {
        return View(new ErrorViewModel { ErrorId = errorId, StatusCode = code });
    }

    // Demo helper for the error mechanism: visit /Home/Crash in Development to force an exception.
    [AllowAnonymous]
    public IActionResult Crash()
    {
        if (!_environment.IsDevelopment())
        {
            return NotFound();
        }

        throw new InvalidOperationException("Test exception to demonstrate the error handling.");
    }
}
