using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace BlazorTemplate.Client.Registrars;

internal interface IWebAssemblyHostBuilderRegistrar : IRegistrar
{
    void RegisterServices(WebAssemblyHostBuilder builder);
}