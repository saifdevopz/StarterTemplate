using Common.Application.Caching;
using Common.Application.Clock;
using Common.Application.Database;
using Common.Application.EventBus;
using Common.Infrastructure.Authentication;
using Common.Infrastructure.Authorization;
using Common.Infrastructure.Caching;
using Common.Infrastructure.Clock;
using Common.Infrastructure.Database;
using Common.Infrastructure.Interceptors;
using Common.Infrastructure.Mail;
using Common.Infrastructure.System;
using Dapper;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Npgsql;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Quartz;
using StackExchange.Redis;

namespace Common.Infrastructure;

public static class InfrastructureConfiguration
{
    public static IServiceCollection AddCommonInfrastructure(
        this IServiceCollection services,
        string serviceName,
        Action<IRegistrationConfigurator, string>[] moduleConfigureConsumers,
        //RabbitMqSettings rabbitMqSettings,
        string redisConnectionString)
    {
        // Mail
        services.ConfigureMailing();

        //Authentication & Authorization
        services.AddAuthenticationInternal();
        services.AddAuthorizationInternal();

        //Services
        services.AddSingleton<ISystemService, SystemService>();
        services.AddTransient<CurrentTenant>();

        services.AddScoped<IDbConnectionFactory, DbConnectionFactory>();
        services.TryAddSingleton<IDateTimeProvider, DateTimeProvider>();
        services.TryAddSingleton<IEventBus, Events.EventBus>();

        //Interceptors
        services.TryAddSingleton<InsertOutboxMessagesInterceptor>();

        //Dapper
        SqlMapper.AddTypeHandler(new GenericArrayHandler<string>());

        //Quartz
        services.AddQuartz(configurator =>
        {
            Guid scheduler = Guid.NewGuid();
            configurator.SchedulerId = $"default-id-{scheduler}";
            configurator.SchedulerName = $"default-name-{scheduler}";
        });

        services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);

        //Redis
        try
        {
            IConnectionMultiplexer connectionMultiplexer = ConnectionMultiplexer.Connect(redisConnectionString);
            services.AddSingleton(connectionMultiplexer);
            services.AddStackExchangeRedisCache(options =>
                    options.ConnectionMultiplexerFactory = () => Task.FromResult(connectionMultiplexer));
        }
        catch
        {
            services.AddDistributedMemoryCache();
        }

        services.TryAddSingleton<ICacheService, CacheService>();

        services.AddMassTransit(configure =>
        {
            string instanceId = serviceName.ToUpperInvariant().Replace('.', '-');
            foreach (Action<IRegistrationConfigurator, string> configureConsumers in moduleConfigureConsumers)
            {
                configureConsumers(configure, instanceId);
            }

            configure.SetKebabCaseEndpointNameFormatter();
            configure.UsingInMemory((context, configure) =>
            {
                configure.ConfigureEndpoints(context);
            });

            //configure.UsingRabbitMq((context, cfg) =>
            //{
            //    cfg.Host(new Uri(rabbitMqSettings.Host), h =>
            //    {
            //        h.Username(rabbitMqSettings.UserName);
            //        h.Password(rabbitMqSettings.Password);
            //        h.MaxMessageSize(500000);
            //    });

            //    cfg.ConfigureEndpoints(context);
            //});
        });

        // OpenTelemetry
        services
                .AddOpenTelemetry()
                .ConfigureResource(resource => resource.AddService(serviceName))
                .WithTracing(tracing =>
                {
                    tracing
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddEntityFrameworkCoreInstrumentation()
                        .AddRedisInstrumentation()
                        .AddNpgsql()
                        .AddSource(MassTransit.Logging.DiagnosticHeaders.DefaultListenerName);

                    //tracing.AddOtlpExporter();
                })
                .WithMetrics(metrics => metrics
                    .AddHttpClientInstrumentation()
                    .AddAspNetCoreInstrumentation()
                    .AddRuntimeInstrumentation())
                .UseOtlpExporter();


        return services;
    }
}