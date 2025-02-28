using BlazorTemplate.Common;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace BlazorTemplate.Client.Registrars;

internal sealed class ClientLibraryRegistrar : IWebAssemblyHostBuilderRegistrar
{
    public void RegisterServices(WebAssemblyHostBuilder builder)
    {
        builder.Services.AddClientLibrary();
    }
}

