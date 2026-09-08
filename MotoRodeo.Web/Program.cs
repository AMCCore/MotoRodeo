using DMCorp.Framework.Basics.DAL;
using DMCorp.Framework.Basics.Security;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using MotoRodeo.BL;
using MotoRodeo.DAL;
using MotoRodeo.DAL.Context;
using MotoRodeo.Web.Health;
using MotoRodeo.Web.Services;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

var ruCulture = new CultureInfo("ru-RU");
CultureInfo.DefaultThreadCurrentCulture = ruCulture;
CultureInfo.DefaultThreadCurrentUICulture = ruCulture;
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new RequestCulture(ruCulture);
    options.SupportedCultures = [ruCulture];
    options.SupportedUICultures = [ruCulture];
});

var configuration = builder.Configuration;

#if DEBUG

Environment.SetEnvironmentVariable("DbConnection", configuration.GetValue<string>("DbConnection"));
Environment.SetEnvironmentVariable("SecKey", configuration.GetValue<string>("SecKey"));
Environment.SetEnvironmentVariable("LuckypennyLicenseKey", configuration.GetValue<string>("LuckypennyLicenseKey"));
Environment.SetEnvironmentVariable("RegistrationClosesDaysBefore", configuration.GetValue<string>("RegistrationClosesDaysBefore"));

Environment.SetEnvironmentVariable("AdminAccountId", configuration.GetValue<string>("AdminAccountId"));
Environment.SetEnvironmentVariable("AdminAccountLogin", configuration.GetValue<string>("AdminAccountLogin"));
Environment.SetEnvironmentVariable("AdminAccountPass", configuration.GetValue<string>("AdminAccountPass"));

Environment.SetEnvironmentVariable("SmtpHost", configuration.GetValue<string>("SmtpHost"));
Environment.SetEnvironmentVariable("SmtpPort", configuration.GetValue<string>("SmtpPort"));
Environment.SetEnvironmentVariable("SmtpUser", configuration.GetValue<string>("SmtpUser"));
Environment.SetEnvironmentVariable("SmtpPassword", configuration.GetValue<string>("SmtpPassword"));
Environment.SetEnvironmentVariable("SmtpFrom", configuration.GetValue<string>("SmtpFrom"));
Environment.SetEnvironmentVariable("SmtpFromName", configuration.GetValue<string>("SmtpFromName"));
Environment.SetEnvironmentVariable("SmtpUseSsl", configuration.GetValue<string>("SmtpUseSsl"));

Environment.SetEnvironmentVariable("EventParticipationApiKey", configuration.GetValue<string>("EventParticipationApiKey"));

#endif


builder.Services.AddControllersWithViews(options =>
{
});
builder.Services.AddHttpContextAccessor();

builder.Services.AddDbContext<MotoRodeoContext>(options => options.UseLazyLoadingProxies()
.UseNpgsql(
        Environment.GetEnvironmentVariable("DbConnection") ?? throw new ArgumentNullException("DbConnection")
    //opts => opts.EnableRetryOnFailure()
    ));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddMotoRodeoBl();
builder.Services.AddScoped<IAdvancedSecurityService, SecurityService>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, x =>
    {
        x.Cookie.HttpOnly = true;
        x.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        x.Cookie.SameSite = SameSiteMode.Strict;
        x.LoginPath = "/Account/Login";
        x.AccessDeniedPath = "/Account/Login";
        x.SlidingExpiration = true;
        x.ExpireTimeSpan = TimeSpan.FromHours(12);
    });

//health checks
builder.Services.AddHealthChecks()
    .AddCheck<SimpleHealthCheck>("simple_check", tags: ["Liveness"])
    .AddCheck<SimpleDbCheck>("simple_db_check", tags: ["Readiness"])
    .AddCheck<DbMigrationsHealthCheck>("simple_db_migration_check", tags: ["Readiness"])
    .AddCheck<SMTPHealthChecks>("smtp_check", tags: ["Readiness"]);

//---------------------------------------------
var app = builder.Build();

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRequestLocalization();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapStaticAssets();
app.MapControllers();
app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapHealthChecks("/health", new HealthCheckOptions
{
    Predicate = r => r.Tags.Contains("Liveness")
});
app.MapHealthChecks("/health/ready");

app.Run();