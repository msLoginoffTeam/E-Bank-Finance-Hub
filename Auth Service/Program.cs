using Auth_Service.Models;
using Auth_Service.Services;
using Auth_Service.Services.Utils;
using Common;
using Common.ErrorHandling;
using Microsoft.EntityFrameworkCore;
using UserApi.Services.Utils;

var builder = WebApplication.CreateBuilder(args);


builder.Configuration.SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("authappsettings.json", optional: true, reloadOnChange: true);
// Add services to the container.
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<GlobalExceptionFilter>(); 
});

builder.Services.AddDbContext<AppDBContext>(options => options.UseNpgsql(Environment.GetEnvironmentVariable("AUTH_DATABASE_CONNECTION") != null ? Environment.GetEnvironmentVariable("AUTH_DATABASE_CONNECTION") : builder.Configuration.GetConnectionString("DataBase")));
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<TokenGenerator>();
builder.Services.AddSingleton<AuthRabbit>();
builder.Services.AddCustomAuthentication();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ResourceAccess", policy => policy.RequireClaim("TokenType", "Access"));
    options.AddPolicy("RefreshTokenAccess", policy => policy.RequireClaim("TokenType", "Refresh"));

    options.DefaultPolicy = options.GetPolicy("ResourceAccess");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDBContext>();
    db.Database.Migrate();

    var bus = app.Services.GetRequiredService<AuthRabbit>();
    bus = new AuthRabbit(app.Services);
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{IsClient?}/{returnUrl?}");

app.Run();
