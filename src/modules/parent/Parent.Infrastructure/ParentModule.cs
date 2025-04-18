using Common.Application.Database;
using Common.Application.EventBus;
using Common.Application.Messaging;
using Common.Infrastructure.Database;
using Common.Infrastructure.Interceptors;
using Common.Infrastructure.System;
using Common.Presentation.Endpoints;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Parent.Infrastructure.Common.Database;
using Parent.Infrastructure.Common.Inbox;
using Parent.Infrastructure.Common.Outbox;
using Store.IntegrationEvents;
using System.Reflection;

namespace Parent.Infrastructure;

public static class ParentModule
{
    public static IServiceCollection AddParentModule(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        services.AddInfrastructure(configuration);

        services.AddDomainEventHandlers();

        services.AddIntegrationEventHandlers();

        services.AddEndpoints(Presentation.AssemblyReference.Assembly);

        return services;
    }

    public static void ConfigureConsumers(IRegistrationConfigurator registrationConfigurator, string instanceId)
    {
        ArgumentNullException.ThrowIfNull(registrationConfigurator);

        registrationConfigurator.AddConsumer<IntegrationEventConsumer<OrderCreatedIntegrationEvent>>()
                                .Endpoint(c => c.InstanceId = instanceId);
    }

#pragma warning disable S1172 // Unused method parameters should be removed
    private static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
#pragma warning restore S1172 // Unused method parameters should be removed
    {
        string defaultContextName = Assembly.GetCallingAssembly().GetName().Name!;
        services.AddTransient<IDbContextProvider>(sp =>
        {
            return new DbContextProvider(sp, defaultContextName);
        });

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        services.AddDbContext<ParentDbContext>((sp, options) =>
        {
            CurrentTenant tenantProvider = sp.GetRequiredService<CurrentTenant>();
            string connectionString = tenantProvider.GetParentConnectionString();

            options.UseSnakeCaseNamingConvention();
            options.UseNpgsql(connectionString, npgsqlOptionsAction =>
            {
                npgsqlOptionsAction.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(2),
                        errorCodesToAdd: null);

                npgsqlOptionsAction.MigrationsHistoryTable(HistoryRepository.DefaultTableName, Constants.Schema);
            });

            options.AddInterceptors(sp.GetRequiredService<InsertOutboxMessagesInterceptor>());
        });

        //services.Configure<OutboxOptions>(configuration.GetSection("Events:Outbox"));
        //services.ConfigureOptions<ConfigureProcessOutboxJob>();

        //services.Configure<InboxOptions>(configuration.GetSection("Events:Inbox"));
        //services.ConfigureOptions<ConfigureProcessInboxJob>();
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


