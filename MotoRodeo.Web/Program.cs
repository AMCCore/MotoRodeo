using DMCorp.Framework.Basics.DAL;
using DMCorp.Framework.Basics.Security;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using MotoRodeo.BL;
using MotoRodeo.BL.Jobs;
using MotoRodeo.DAL;
using MotoRodeo.DAL.Context;
using MotoRodeo.Web.Filters;
using MotoRodeo.Web.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<DomainExceptionFilter>();
});
builder.Services.AddHttpContextAccessor();
builder.Services.AddMotoRodeoDal(builder.Configuration);
builder.Services.AddMotoRodeoBl();
builder.Services.Configure<EventOptions>(builder.Configuration.GetSection(EventOptions.SectionName));
builder.Services.AddScoped<IAdvancedSecurityService, SecurityService>();
builder.Services.AddHostedService<CloseRegistrationHostedService>();

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

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<MotoRodeoContext>();
    await db.Database.MigrateAsync();
    if (app.Environment.IsDevelopment())
    {
        var seed = app.Configuration.GetSection("Seed");
        var uw = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        uw.SeedData(
            seed["AdminLogin"] ?? "admin",
            seed["AdminPassword"] ?? "admin123",
            seed["AdminName"] ?? "Администратор");
    }
}

app.Run();