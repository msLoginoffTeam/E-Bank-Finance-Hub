using System.Reflection;
using System.Text.Json.Serialization;
using CreditService_Patterns.Contexts;
using CreditService_Patterns.IServices;
using CreditService_Patterns.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Quartz;
using Common;
using CreditService_Patterns.Services.Utils;
using UserApi.Services.Utils;
using Common.Idempotency;
using Common.InternalServerErrorMiddleware;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("creditappsettings.Development.json", optional: true, reloadOnChange: true);

builder.Services.AddDbContext<CreditServiceContext>(options => options.UseNpgsql(Environment.GetEnvironmentVariable("CREDIT_DATABASE_CONNECTION") != null ? Environment.GetEnvironmentVariable("CREDIT_DATABASE_CONNECTION") : builder.Configuration.GetConnectionString("CreditServiceContext")));

builder.Services.AddControllers().AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
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

builder.Services.AddScoped<ICreditService, CreditService>();
builder.Services.AddSingleton<CreditRabbit>();
builder.Services.AddCustomAuthentication();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ResourceAccess", policy => policy.RequireClaim("TokenType", "Access"));
    options.DefaultPolicy = options.GetPolicy("ResourceAccess");
});

builder.Services.AddQuartz(q =>
{
    var jobKey = new JobKey("DailyJob");

    q.AddJob<DailyJobService>(opts => opts.WithIdentity(jobKey));

    q.AddTrigger(opts => opts
        .ForJob(jobKey)
        .WithIdentity("DailyTrigger")
        .WithCronSchedule("*/20 * * * * ?")
    );
});

builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var CreditServiceContext = scope.ServiceProvider.GetRequiredService<CreditServiceContext>();
    await CreditServiceContext.Database.MigrateAsync();

    var rabbit = app.Services.GetRequiredService<CreditRabbit>();
    rabbit = new CreditRabbit(app.Services);
}
app.UseCors("AllowAllOrigins");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseMiddleware<HttpInstabilityMiddleware>();

app.UseMiddleware<IdempotencyMiddleware>();

app.MapControllers();

app.Run();
