using DMCorp.Framework.Basics.DAL;
using DMCorp.Framework.Basics.Security;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using MotoRodeo.BL;
using MotoRodeo.DAL;
using MotoRodeo.DAL.Context;
using MotoRodeo.Web.Filters;
using MotoRodeo.Web.Services;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

#if DEBUG

Environment.SetEnvironmentVariable("DbConnection", configuration.GetValue<string>("DbConnection"));
Environment.SetEnvironmentVariable("SecKey", configuration.GetValue<string>("SecKey"));
Environment.SetEnvironmentVariable("LuckypennyLicenseKey", configuration.GetValue<string>("LuckypennyLicenseKey"));
Environment.SetEnvironmentVariable("DefaultRegistrationClosesDaysBefore", configuration.GetValue<string>("DefaultRegistrationClosesDaysBefore"));
Environment.SetEnvironmentVariable("AdminLogin", configuration.GetValue<string>("AdminLogin"));
Environment.SetEnvironmentVariable("AdminPassword", configuration.GetValue<string>("AdminPassword"));
Environment.SetEnvironmentVariable("AdminName", configuration.GetValue<string>("AdminName"));
Environment.SetEnvironmentVariable("SmtpHost", configuration.GetValue<string>("SmtpHost"));
Environment.SetEnvironmentVariable("SmtpPort", configuration.GetValue<string>("SmtpPort"));
Environment.SetEnvironmentVariable("SmtpUser", configuration.GetValue<string>("SmtpUser"));
Environment.SetEnvironmentVariable("SmtpPassword", configuration.GetValue<string>("SmtpPassword"));
Environment.SetEnvironmentVariable("SmtpFrom", configuration.GetValue<string>("SmtpFrom"));
Environment.SetEnvironmentVariable("SmtpFromName", configuration.GetValue<string>("SmtpFromName"));
Environment.SetEnvironmentVariable("SmtpUseSsl", configuration.GetValue<string>("SmtpUseSsl"));
Environment.SetEnvironmentVariable("EmailTemplatePasswordReset", configuration.GetValue<string>("EmailTemplatePasswordReset"));
Environment.SetEnvironmentVariable("EmailTemplateNewPassword", configuration.GetValue<string>("EmailTemplateNewPassword"));
Environment.SetEnvironmentVariable("EmailTemplateRegistration", configuration.GetValue<string>("EmailTemplateRegistration"));
Environment.SetEnvironmentVariable("EmailSubjectPasswordReset", configuration.GetValue<string>("EmailSubjectPasswordReset"));
Environment.SetEnvironmentVariable("EmailSubjectNewPassword", configuration.GetValue<string>("EmailSubjectNewPassword"));
Environment.SetEnvironmentVariable("EmailSubjectRegistration", configuration.GetValue<string>("EmailSubjectRegistration"));

#endif


builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<DomainExceptionFilter>();
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

//---------------------------------------------
var app = builder.Build();

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapStaticAssets();
app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();