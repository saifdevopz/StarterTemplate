using Common.Application.Database;
using Common.Application.EventBus;
using Common.Application.Messaging;
using Common.Infrastructure.Database;
using Common.Infrastructure.Interceptors;
using Common.Infrastructure.System;
using Common.Presentation.Endpoints;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Store.Infrastructure.Common.Database;
using Store.Infrastructure.Common.Inbox;
using Store.Infrastructure.Common.Outbox;
using System.Reflection;

namespace Store.Infrastructure;

public static class StoreModule
{
    public static IServiceCollection AddStoreModule(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        services.AddInfrastructure();

        services.AddDomainEventHandlers();

        services.AddIntegrationEventHandlers();

        services.AddEndpoints(Presentation.AssemblyReference.Assembly);

        return services;
    }

    private static void AddInfrastructure(this IServiceCollection services)
    {
        string defaultContextName = Assembly.GetCallingAssembly().GetName().Name!;
        services.AddScoped<IDbContextProvider>(sp =>
        {
            return new DbContextProvider(sp, defaultContextName);
        });

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        services.AddDbContext<StoreDbContext>((sp, options) =>
        {
            CurrentTenant tenantProvider = sp.GetRequiredService<CurrentTenant>();
            string connectionString = tenantProvider.GetStoreConnectionString();

            options.AddInterceptors(sp.GetServices<InsertOutboxMessagesInterceptor>());
            options.UseSqlServer(connectionString, sqlServerOptionsAction: sqlOptions =>
            {
                sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 10,
                maxRetryDelay: TimeSpan.FromSeconds(5),
                errorNumbersToAdd: null);
            });
        });

        //services.Configure<OutboxOptions>(configuration.GetSection("Store:Outbox"));
        //services.ConfigureOptions<ConfigureProcessOutboxJob>();

        //services.Configure<InboxOptions>(configuration.GetSection("Store:Inbox"));
        //services.ConfigureOptions<ConfigureProcessInboxJob>();
    }

    public static void ConfigureConsumers(IRegistrationConfigurator registrationConfigurator, string instanceId)
    {
        ArgumentNullException.ThrowIfNull(registrationConfigurator);

        //registrationConfigurator.
        //    AddConsumer<GetUserPermissionsRequestConsumer>()
        //        .Endpoint(c => c.InstanceId = instanceId);
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


