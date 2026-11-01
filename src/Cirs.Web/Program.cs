using Cirs.Web.Configuration;
using Cirs.Web.Data;
using Cirs.Web.Middleware;
using Cirs.Web.Services;
using Cirs.Web.Validators;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;

// Bootstrap logger so a crash during startup still reaches the console.
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    // The real logger is configured from appsettings.json (level, file path, retention).
    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext());

    builder.Services.Configure<AuthOptions>(builder.Configuration.GetSection(AuthOptions.SectionName));
    var authOptions = builder.Configuration.GetSection(AuthOptions.SectionName).Get<AuthOptions>() ?? new AuthOptions();

    // Validators are the authority on the rules, so the automatic "non-nullable means required" check is switched off.
    builder.Services.AddControllersWithViews(options =>
        options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true);

    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is missing.");

    // The server version is stated here so startup does not need a live database connection.
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 36))));

    builder.Services.AddScoped<IAppLogger, AppLogger>();
    builder.Services.AddScoped<IAuthService, AuthService>();
    builder.Services.AddSingleton<UserValidator>();

    builder.Services
        .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddCookie(options =>
        {
            options.LoginPath = "/Account/Login";
            options.AccessDeniedPath = "/Account/AccessDenied";
            options.Cookie.Name = "Cirs.Auth";
            options.Cookie.HttpOnly = true;
            options.Cookie.SameSite = SameSiteMode.Lax;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            options.ExpireTimeSpan = TimeSpan.FromMinutes(authOptions.CookieExpireMinutes);
            options.SlidingExpiration = true;
        });

    // The anti-forgery cookie is HTTPS only as well. Run the app on the https profile (https://localhost:7080).
    builder.Services.AddAntiforgery(options =>
    {
        options.Cookie.Name = "Cirs.Antiforgery";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Strict;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    });

    builder.Services.AddDistributedMemoryCache();
    builder.Services.AddSession(options =>
    {
        options.Cookie.Name = "Cirs.Session";
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.IdleTimeout = TimeSpan.FromMinutes(authOptions.SessionIdleMinutes);
    });

    var app = builder.Build();

    AuthService.WarmUp(authOptions.BcryptWorkFactor);

    // Request logging sits outside the exception middleware so handled errors are not logged twice.
    app.UseSerilogRequestLogging(options =>
        options.GetLevel = (httpContext, _, exception) =>
            exception is not null || httpContext.Response.StatusCode >= 500 ? LogEventLevel.Error
            : httpContext.RequestAborted.IsCancellationRequested ? LogEventLevel.Debug
            : httpContext.Request.Path.StartsWithSegments("/css")
              || httpContext.Request.Path.StartsWithSegments("/js")
              || httpContext.Request.Path.StartsWithSegments("/lib") ? LogEventLevel.Verbose
            : LogEventLevel.Information);

    app.UseMiddleware<ExceptionHandlingMiddleware>();

    if (!app.Environment.IsDevelopment())
    {
        app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();
    app.UseStatusCodePagesWithReExecute("/Home/Error", "?code={0}");

    app.UseRouting();
    app.UseSession();
    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

    // One line in the log tells us right away whether the database is reachable.
    using (var scope = app.Services.CreateScope())
    {
        var appLogger = scope.ServiceProvider.GetRequiredService<IAppLogger>();

        try
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            if (await db.Database.CanConnectAsync())
            {
                appLogger.Info("Application started. Database connection OK.");
            }
            else
            {
                appLogger.Warning("Application started, but the database is not reachable. Check the connection string and that cirs_db exists.");
            }
        }
        catch (Exception ex)
        {
            appLogger.Error(ex, "Application started, but the database check failed.");
        }
    }

    app.Run();
}
catch (Exception ex) when (ex is not HostAbortedException)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
