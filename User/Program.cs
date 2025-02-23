using System.Text;
using System.Text.Json.Serialization;
using Core.Services.Utils.ErrorHandling;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using UserApi.Data;
using UserApi.Services;
using UserApi.Services.Utils.TokenGenerator;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDBContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DataBase")));

builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<TokenGenerator>();

TokenGeneratorConfiguration tokenGeneratorConfiguration = new TokenGeneratorConfiguration();
builder.Configuration.Bind("Authentication", tokenGeneratorConfiguration);
builder.Services.AddSingleton(tokenGeneratorConfiguration);

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;

        options.TokenValidationParameters = new TokenValidationParameters()
        {
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenGeneratorConfiguration.AccessTokenSecret)),
            ValidIssuer = tokenGeneratorConfiguration.Issuer,
            ValidAudience = tokenGeneratorConfiguration.Audience,
            ValidateIssuerSigningKey = true,
            ValidateIssuer = true,
            ValidateAudience = true
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ResourceAccess", policy => policy.RequireClaim("TokenType", "Access"));
    options.AddPolicy("RefreshTokenAccess", policy => policy.RequireClaim("TokenType", "Refresh"));

    options.DefaultPolicy = options.GetPolicy("ResourceAccess");
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDBContext>();
    db.Database.Migrate();
}
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseMiddleware<ErrorHandlingMiddleware>();

app.MapControllers();

app.Run();
