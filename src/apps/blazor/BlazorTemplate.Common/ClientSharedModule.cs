using Blazored.LocalStorage;
using BlazorTemplate.Common.Authentication;
using BlazorTemplate.Common.Delegates;
using BlazorTemplate.Common.Helpers;
using BlazorTemplate.Common.Services.Contracts;
using BlazorTemplate.Common.Services.Implementations;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Syncfusion.Blazor;

namespace BlazorTemplate.Common;

public static class ClientSharedModule
{
    public static IServiceCollection AddClientLibrary(this IServiceCollection services)
    {
        // Syncfusion
        Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MzcyNTU1MEAzMjM4MmUzMDJlMzBZamQrN042Yi9pQ0lVQWxZa216cFdEY2k5SkJRdlI3aDJxaUtYR20zQkFjPQ==");
        services.AddSyncfusionBlazor();

        // Authorization
        services.AddAuthorizationCore();
        services.AddScoped<CustomAuthenticationStateProvider>();
        services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();

        // Local Storage
        services.AddBlazoredLocalStorage();
        services.AddScoped<LocalStorageService>();

        // Http Client
        services.AddScoped<CustomHttpClient>();

        // Middleware & Delegates
        services.AddTransient<ErrorHandlerDelegate>();
        services.AddTransient<TokenDelegate>();

        // Services
        services.AddScoped<ITokenService, TokenService>();
        //services.AddScoped<IGenericService<GetAllTenants, CreateTenantDto>, GenericService<GetAllTenants, CreateTenantDto>>();
        //services.AddScoped<IGenericService<ReadTenantTypeDto, WriteTenantType>, GenericService<ReadTenantTypeDto, WriteTenantType>>();

        return services;
    }
}
