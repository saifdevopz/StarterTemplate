using Common.Application.Mail;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Infrastructure.Mail;

internal static class MailExtension
{
    internal static IServiceCollection ConfigureMailing(this IServiceCollection services)
    {
        services.AddTransient<IMailService, MailService>();
        services.AddOptions<MailOptions>().BindConfiguration(nameof(MailOptions));
        return services;
    }
}