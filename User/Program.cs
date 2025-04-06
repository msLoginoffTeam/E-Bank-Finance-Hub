using System.Reflection;
using System.Text.Json.Serialization;
using Common;
using Common.ErrorHandling;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using User_Api.Data.DTOs.Responses;
using UserApi.Data;
using UserApi.Services;
using UserApi.Services.Utils;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("userappsettings.json", optional: true, reloadOnChange: true);

builder.Services.AddControllers().AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter())).ConfigureApiBehaviorOptions(options =>
        {
            options.InvalidModelStateResponseFactory = InvalidModelResponse.MakeValidationResponse;
        }); ;
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder => builder.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader());
});

builder.Services.AddSwaggerGen(options =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new List<string>()
        }
    });
});

builder.Services.AddDbContext<AppDBContext>(options => options.UseNpgsql(Environment.GetEnvironmentVariable("USER_DATABASE_CONNECTION") != null ? Environment.GetEnvironmentVariable("USER_DATABASE_CONNECTION") : builder.Configuration.GetConnectionString("DataBase")));
builder.Services.AddScoped<UserService>();
builder.Services.AddSingleton<UserRabbit>();
builder.Services.AddCustomAuthentication();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ResourceAccess", policy => policy.RequireClaim("TokenType", "Access"));
    options.DefaultPolicy = options.GetPolicy("ResourceAccess");
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDBContext>();
    db.Database.Migrate();

    var rabbit = app.Services.GetRequiredService<UserRabbit>();
    rabbit = new UserRabbit(app.Services);

}
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAllOrigins");

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseMiddleware<ErrorHandlingMiddleware>();

app.MapControllers();

app.Run();
