using CreditService_Patterns.Contexts;
using CreditService_Patterns.IServices;
using CreditService_Patterns.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<CreditServiceContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("CreditServiceContext")));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<ICreditService, CreditService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var CreditServiceContext = scope.ServiceProvider.GetRequiredService<CreditServiceContext>();
    await CreditServiceContext.Database.MigrateAsync();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
