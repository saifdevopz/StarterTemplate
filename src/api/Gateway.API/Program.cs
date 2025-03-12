using Common.Application;
using Common.Infrastructure;
using Common.Infrastructure.Configuration;
using Common.Presentation.Endpoints;
using Gateway.API.Extensions;
using Gateway.API.Middleware;
using Gateway.API.OpenTelemetry;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Scalar.AspNetCore;
using Serilog;
using System.Infrastructure;
using System.Reflection;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Connection Strings
string systemConnectionString = builder.Configuration.GetValueOrThrow<string>("SQLServer:DefaultConnection");
string redisConnectionString = builder.Configuration.GetValueOrThrow<string>("Redis:DefaultConnection");
//RabbitMqSettings rabbitMqSettings = builder.Configuration.GetSection("RabbitMQ").Get<RabbitMqSettings>()!;

builder.Host.UseSerilog((context, loggerConfig) => loggerConfig.ReadFrom.Configuration(context.Configuration));

// Global Exception Handling
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// Controller Support
builder.Services.AddControllers();

// Minimal API Support
builder.Services.AddEndpointsApiExplorer();

// Application Module Assemblies
Assembly[] moduleApplicationAssemblies =
[
    System.Application.AssemblyReference.Assembly,
];

// Common Application Module
builder.Services.AddCommonApplication(moduleApplicationAssemblies);

// Common Infrastructure Module
builder.Services.AddCommonInfrastructure(
    DiagnosticsConfig.ServiceName,
    [
        SystemModule.ConfigureConsumers,
    ],
    //rabbitMqSettings,
    //systemConnectionString,
    redisConnectionString
    );

// Adding Other Modules
builder.Services.AddSystemModule(builder.Configuration, systemConnectionString);

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.Services
    .AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService(DiagnosticsConfig.ServiceName))
    .WithTracing(tracing =>
    {
        tracing
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddSource("Yarp.ReverseProxy");

        tracing.AddOtlpExporter();
    });

//builder.Services.AddAuthorization();

//builder.Services.AddAuthentication().AddJwtBearer();

//builder.Services.ConfigureOptions<JwtBearerConfigureOptions>();

// API Documentation
builder.Services.AddOpenApi();

builder.Services.AddHealthChecks()
    .AddSqlServer(systemConnectionString)
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

app.MapControllers();

app.UseCors("MyPolicy");

if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.MapOpenApi();
    app.MapScalarApiReference(_ =>
    {
        _.Servers = [];
        _.Theme = ScalarTheme.Kepler;
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

app.MapReverseProxy();

await app.RunAsync();


