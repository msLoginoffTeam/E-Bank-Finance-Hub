using Common.Trace;
using EasyNetQ;
using Microsoft.EntityFrameworkCore;
using TraceService.Data;
using TraceService.IServices;
using TraceService.Services;
using TraceService.Services.Utils;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.SetBasePath(Directory.GetCurrentDirectory())
				   .AddJsonFile("traceappsettings.json", optional: true, reloadOnChange: true);
builder.Services.AddLogging();
builder.Services.AddControllersWithViews();

builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowAllOrigins",
		builder => builder.AllowAnyOrigin()
						  .AllowAnyMethod()
						  .AllowAnyHeader());
});

builder.Services.AddDbContext<TraceContext>(options =>
{
	options
		.UseNpgsql(
			Environment.GetEnvironmentVariable("TRACE_DATABASE_CONNECTION") != null
				? Environment.GetEnvironmentVariable("TRACE_DATABASE_CONNECTION")
				: builder.Configuration.GetConnectionString("DataBase")
		)
		.EnableSensitiveDataLogging();
});
builder.Services.AddScoped<ITraceBdService, TraceBdService>();
builder.Services.AddSingleton<TraceRabbit>();
builder.Services.AddSingleton<Tracer>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
	var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

	try
	{
		var db = scope.ServiceProvider.GetRequiredService<TraceContext>();
		db.Database.Migrate();
	}
	catch (Exception ex)
	{
		logger.LogError(ex, "Ошибка при миграции базы данных");
		throw;
	}
	var loggerr = app.Services.GetRequiredService<ILogger<TraceRabbit>>();

	var rabbit = app.Services.GetRequiredService<TraceRabbit>();
	rabbit = new TraceRabbit(app.Services, loggerr);

	rabbit.Configure();

	Console.WriteLine("RabbitMQ configured successfully.");
}

if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Trace}/{action=GetLogs}");

app.Run();
