using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using System.API.Infrastructure.Database;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Connection Strings
string? systemDatabaseString = builder.Configuration["SQLServer:DefaultConnection"];
ArgumentException.ThrowIfNullOrWhiteSpace(systemDatabaseString);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddDbContext<SystemDbContext>((sp, options) =>
{
    options.UseSqlServer(systemDatabaseString, sqlServerOptionsAction: sqlOptions =>
    {
        sqlOptions.EnableRetryOnFailure(
        maxRetryCount: 10,
        maxRetryDelay: TimeSpan.FromSeconds(5),
        errorNumbersToAdd: null);
    });
});

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(_ =>
    {
        _.Servers = [];
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
