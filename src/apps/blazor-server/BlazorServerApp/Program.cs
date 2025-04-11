using Blazored.LocalStorage;
using BlazorServerApp.Authentication;
using BlazorServerApp.Components;
using BlazorServerApp.Helpers;
using BlazorServerApp.HttpClients;
using BlazorServerApp.Middlewares;
using BlazorServerApp.Services.Contracts;
using BlazorServerApp.Services.Implementations;
using Microsoft.AspNetCore.Components.Authorization;
using Syncfusion.Blazor;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Syncfusion
Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1NNaF5cXmBCf1FpRmJGdld5fUVHYVZUTXxaS00DNHVRdkdmWXtedHZXRGZcVkVxWkBWYUA=");
builder.Services.AddSyncfusionBlazor();

builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<CustomAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();

// Local Storage
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<LocalStorageService>();

// Http Client
builder.Services.AddScoped<CustomHttpClient>();

// Middleware & Delegates
//builder.Services.AddTransient<ErrorHandlerDelegate>();
builder.Services.AddTransient<TokenDelegate>();

// Services
builder.Services.AddScoped<ITokenService, TokenService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
