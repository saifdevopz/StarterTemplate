using Common.Application.Authorization;
using Common.Application.Database;
using Common.Application.EventBus;
using Common.Application.Messaging;
using Common.Infrastructure.Database;
using Common.Infrastructure.Interceptors;
using Common.Presentation.Endpoints;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Application.Common.Interfaces;
using System.Infrastructure.Common.Authentication;
using System.Infrastructure.Common.Authorization;
using System.Infrastructure.Common.Database;
using System.Infrastructure.Common.Inbox;
using System.Infrastructure.Common.Outbox;
using System.Infrastructure.Common.Repository;
using System.Presentation.Consumers;
using System.Reflection;

namespace System.Infrastructure;

public static class SystemModule
{
    public static IServiceCollection AddSystemModule(
        this IServiceCollection services,
        IConfiguration configuration,
        string systemDatabaseString)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        services.AddDomainEventHandlers();

        services.AddIntegrationEventHandlers();

        services.AddInfrastructure(configuration, systemDatabaseString);

        services.AddEndpoints(Presentation.AssemblyReference.Assembly);

        return services;
    }

    private static void AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        string systemDatabaseString)
    {
        services.AddScoped<ITenantService, TenantService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IPermissionService, PermissionService>();

        string defaultContextName = Assembly.GetCallingAssembly().GetName().Name!;
        services.AddScoped<IDbContextProvider>(sp =>
        {
            return new DbContextProvider(sp, defaultContextName);
        });

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        services.AddDbContext<SystemDbContext>((sp, options) =>
        {
            options.AddInterceptors(sp.GetServices<InsertOutboxMessagesInterceptor>());
            options.UseSqlServer(systemDatabaseString, sqlServerOptionsAction: sqlOptions =>
            {
                sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 10,
                maxRetryDelay: TimeSpan.FromSeconds(5),
                errorNumbersToAdd: null);
            });
        });

        services.AddTransient<DataSeeder>();

        services.Configure<OutboxOptions>(configuration.GetSection("Events:Outbox"));
        services.ConfigureOptions<ConfigureProcessOutboxJob>();

        services.Configure<InboxOptions>(configuration.GetSection("Events:Inbox"));
        services.ConfigureOptions<ConfigureProcessInboxJob>();
    }

    public static void ConfigureConsumers(IRegistrationConfigurator registrationConfigurator, string instanceId)
    {
        ArgumentNullException.ThrowIfNull(registrationConfigurator);

        registrationConfigurator.
            AddConsumer<GetUserPermissionsRequestConsumer>()
                .Endpoint(c => c.InstanceId = instanceId);
    }

    private static void AddDomainEventHandlers(this IServiceCollection services)
    {
        Type[] domainEventHandlers = Application.AssemblyReference.Assembly
                .GetTypes()
                .Where(t => t.IsAssignableTo(typeof(IDomainEventHandler)))
                .ToArray();

        foreach (Type domainEventHandler in domainEventHandlers)
        {
            services.TryAddScoped(domainEventHandler);

            Type domainEvent = domainEventHandler
                    .GetInterfaces()
                    .Single(i => i.IsGenericType)
                    .GetGenericArguments()
                    .Single();

            Type closedIdempotentHandler = typeof(IdempotentDomainEventHandler<>).MakeGenericType(domainEvent);

            services.Decorate(domainEventHandler, closedIdempotentHandler);
        }
    }

    private static void AddIntegrationEventHandlers(this IServiceCollection services)
    {
        Type[] integrationEventHandlers = Presentation.AssemblyReference.Assembly
                .GetTypes()
                .Where(t => t.IsAssignableTo(typeof(IIntegrationEventHandler)))
                .ToArray();

        foreach (Type integrationEventHandler in integrationEventHandlers)
        {
            services.TryAddScoped(integrationEventHandler);

            Type integrationEvent = integrationEventHandler
                    .GetInterfaces()
                    .Single(i => i.IsGenericType)
                    .GetGenericArguments()
                    .Single();

            Type closedIdempotentHandler =
                    typeof(IdempotentIntegrationEventHandler<>).MakeGenericType(integrationEvent);

            services.Decorate(integrationEventHandler, closedIdempotentHandler);
        }
    }

}


