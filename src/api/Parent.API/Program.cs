using Common.Application;
using Common.Infrastructure;
using Common.Infrastructure.Configuration;
using Common.Presentation.Endpoints;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Parent.API.Extensions;
using Parent.API.Middleware;
using Parent.API.OpenTelemetry;
using Parent.Infrastructure;
using Scalar.AspNetCore;
using Serilog;
using System.Reflection;
using System.Text.Json.Serialization;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Connection Strings
string parentConnectionString = builder.Configuration.GetValueOrThrow<string>("PostgreSQL:DefaultConnection");
string redisConnectionString = builder.Configuration.GetValueOrThrow<string>("Redis:DefaultConnection");
//RabbitMqSettings rabbitMqSettings = builder.Configuration.GetSection("RabbitMQ").Get<RabbitMqSettings>()!;

// Controller API Support
builder.Services.AddControllers().AddJsonOptions(x =>
     x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve);

// Serilog
builder.Host.UseSerilog((context, loggerConfig) => loggerConfig.ReadFrom.Configuration(context.Configuration));

// Global Exception Handling
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// Minimal API Support
builder.Services.AddEndpointsApiExplorer();

// Application Module Assemblies
Assembly[] moduleApplicationAssemblies =
[
    Parent.Application.AssemblyReference.Assembly,
];

// Common Application Module
builder.Services.AddCommonApplication(moduleApplicationAssemblies);

// Common Infrastructure Module
builder.Services.AddCommonInfrastructure(
    DiagnosticsConfig.ServiceName,
    [
        ParentModule.ConfigureConsumers,
    ],
    //rabbitMqSettings,
    //parentConnectionString,
    redisConnectionString);

// Adding Other Modules
builder.Services.AddParentModule(builder.Configuration);

// Module Configuration Settings
builder.Configuration.AddModuleConfiguration(["parent"]);

// API Documentation
builder.Services.AddOpenApi();

builder.Services.AddHealthChecks()
    .AddNpgSql(parentConnectionString)
    //.AddRabbitMQ(sp =>
    //{
    //    ConnectionFactory factory = new()
    //    {
    //        HostName = rabbitMqSettings.Host,
    //        UserName = rabbitMqSettings.UserName,
    //        Port = 5672,
    //        Password = rabbitMqSettings.Password,
    //    };
    //    return factory.CreateConnectionAsync();
    //})
    .AddRedis(redisConnectionString);

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("MyPolicy", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

WebApplication app = builder.Build();

app.UseCors("MyPolicy");

app.MapControllers();

if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.MapOpenApi();
    app.MapScalarApiReference(_ =>
    {
        _.Servers = [];
    });

    await app.ApplyAllMigrations();
}

app.MapHealthChecks("health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapHealthChecksUI(options =>
{
    options.UIPath = "/health-ui";
});

app.UseHttpsRedirection();

app.UseLogContextTraceLogging();

app.UseSerilogRequestLogging();

app.UseExceptionHandler();

app.UseAuthentication();

app.UseAuthorization();

app.MapEndpoints();

await app.RunAsync();


