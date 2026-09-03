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

Environment.SetEnvironmentVariable("AdminAccountId", configuration.GetValue<string>("AdminAccountId"));
Environment.SetEnvironmentVariable("AdminAccountLogin", configuration.GetValue<string>("AdminAccountLogin"));
Environment.SetEnvironmentVariable("AdminAccountPass", configuration.GetValue<string>("AdminAccountPass"));

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