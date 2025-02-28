using BlazorTemplate.Common.Delegates;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace BlazorTemplate.Client.Registrars;

internal sealed class BaseAddressRegistrar : IWebAssemblyHostBuilderRegistrar
{
    public void RegisterServices(WebAssemblyHostBuilder builder)
    {
        builder.Services.AddHttpClient("SystemApiClient", (sp, client) =>
        {
            IWebAssemblyHostEnvironment env = builder.HostEnvironment;
            IConfiguration configuration = sp.GetRequiredService<IConfiguration>();

            if (env.IsDevelopment())
            {
                client.BaseAddress = new Uri(configuration["BaseUrls:Development"]!);
            }

            if (env.IsProduction())
            {
                client.BaseAddress = new Uri(configuration["BaseUrls:Production"]!);
            }

        })
            .AddHttpMessageHandler<TokenDelegate>()
            .AddHttpMessageHandler<ErrorHandlerDelegate>();

        builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
    }
}