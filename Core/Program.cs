using Common;
using Common.ErrorHandling;
using Common.Idempotency;
using Common.InternalServerErrorMiddleware;
using Common.Rabbit;
using Common.Trace;
using Core.Data;
using Core.Services;
using Core.Services.Utils;
using Core_Api.Services.Utils;
using Fleck;
using Microsoft.AspNetCore.WebSockets;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;
using System.Reflection;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("coreappsettings.json", optional: true, reloadOnChange: true);

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddDbContext<AppDBContext>(options => options.UseNpgsql(Environment.GetEnvironmentVariable("CORE_DATABASE_CONNECTION") != null ? Environment.GetEnvironmentVariable("CORE_DATABASE_CONNECTION") : builder.Configuration.GetConnectionString("DataBase")));

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

builder.Services.AddScoped<AccountService>();
builder.Services.AddScoped<OperationService>();
builder.Services.AddSingleton<CoreRabbit>();
builder.Services.AddSingleton<Tracer>();
builder.Services.AddHostedService<CurrencyCoursesGetter>();
builder.Services.AddSingleton<WebSocketServerManager>();
builder.Services.AddHostedService<FirebaseNotificator>();
builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(Environment.GetEnvironmentVariable("REDIS_CONNECTION") != null ? Environment.GetEnvironmentVariable("REDIS_CONNECTION") : "localhost"));
Console.WriteLine(Environment.GetEnvironmentVariable("REDIS_CONNECTION"));
builder.Services.AddCustomAuthentication();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ResourceAccess", policy => policy.RequireClaim("TokenType", "Access"));
    options.DefaultPolicy = options.GetPolicy("ResourceAccess");
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDBContext>();
    db.Database.Migrate();

    var webSocket = app.Services.GetRequiredService<WebSocketServerManager>();
    webSocket.Start();

	var tracer = app.Services.GetRequiredService<Tracer>();

	var bus = app.Services.GetRequiredService<CoreRabbit>();
	bus = new CoreRabbit(app.Services, app.Services.GetRequiredService<IConnectionMultiplexer>(), tracer);
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

// if (Environment.GetEnvironmentVariable("USE_INSTABILITY") == "true")
app.UseMiddleware<HttpInstabilityMiddleware>();

app.UseMiddleware<IdempotencyMiddleware>();

app.MapControllers();

app.Run();
