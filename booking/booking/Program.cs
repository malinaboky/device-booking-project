using booking.Entities;
using booking.Services;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json.Serialization;
using Visus.LdapAuthentication; 

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddEntityFrameworkMySql().AddDbContext<DeviceBookingContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"), ServerVersion.Parse("5.5.62-mysql")));

builder.Services.AddTransient<BufferedFileUploadLocalService>();

builder.Services.AddTransient<FileDownloadService>();

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
    options.Cookie.SameSite = SameSiteMode.None;
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

var app = builder.Build();

using (var serviceScope = app.Services.CreateScope())
{
    var context = serviceScope.ServiceProvider.GetRequiredService<DeviceBookingContext>();
    context.Database.EnsureCreated();
}

app.UseCookiePolicy();

// app.UseHttpsRedirection();

// app.UseRouting();

app.UseCors("CorsPolicy");

app.UseAuthorization();

app.UseAuthentication();

app.MapControllers();

app.Run();
