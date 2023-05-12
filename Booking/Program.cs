using Database;
using Database.Services;

using Microsoft.AspNetCore.Authentication.Cookies;
using DotNetEd.CoreAdmin;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json.Serialization;
using Visus.LdapAuthentication;
using DotNetEd.CoreAdmin.Service;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddControllersWithViews();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddEntityFrameworkMySql().AddDbContext<DeviceBookingContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"), ServerVersion.Parse("5.5.62-mysql")));

builder.Services.AddTransient<FileUploadLocalService>();

builder.Services.AddTransient<FileDownloadService>();

builder.Services.AddTransient<DeviceService>();

builder.Services.AddTransient<UserService>();

builder.Services.AddTransient<ReportService>();

builder.Services.AddTransient<RecordService>();

builder.Services.AddControllers().AddJsonOptions(x =>
                x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

builder.Host.ConfigureServices(services =>
{
    var ldap = new LdapOptions();
    builder.Configuration.GetSection("LdapConfiguration").Bind(ldap);
    services.AddLdapAuthenticationService<LdapUser>(ldap);
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
{
    options.SlidingExpiration = true;
    options.Events.OnRedirectToLogin = (context) => {
        context.Response.StatusCode = 401;
        return Task.CompletedTask;
    };
    options.Cookie.HttpOnly = true;
});

builder.Services.AddAuthorization(opts =>
{
    opts.AddPolicy("OnlyForAdmin", policy =>
    {
        policy.RequireClaim(ClaimTypes.Role, "Admin");
    });
});

builder.Services.AddCors(options => {
    options.AddPolicy("CorsPolicy",
        builder => builder
        .WithOrigins("http://localhost:4200", "https://localhost:4200")
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials()
        );
});

builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.Configure<Microsoft.AspNetCore.Mvc.JsonOptions>(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddCoreAdmin();

var app = builder.Build();

using (var serviceScope = app.Services.CreateScope())
{
    var context = serviceScope.ServiceProvider.GetRequiredService<DeviceBookingContext>();
    context.Database.EnsureCreated();
}

app.UseCookiePolicy();

app.MapDefaultControllerRoute();

app.UseCors("CorsPolicy");

app.UseAuthorization();

app.UseAuthentication();

app.MapControllers();

app.UseStaticFiles();

app.Run();
