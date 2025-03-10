using BlazorWasmTemplate;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using TG.Blazor.IndexedDB;
using Toolbelt.Blazor.Extensions.DependencyInjection;

WebAssemblyHostBuilder builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddPWAUpdater();

builder.Services.AddIndexedDB(dbStore =>
{
    dbStore.DbName = "EmployeeDB";
    dbStore.Version = 1;

    dbStore.Stores.Add(new StoreSchema
    {
        Name = "EmployeesData",
        PrimaryKey = new IndexSpec { Name = "id", KeyPath = "id", Unique = true, Auto = true },
        Indexes =
                    [
                        new IndexSpec{Name="firstName", KeyPath = "firstName", Auto=false},
                        new IndexSpec{Name="lastName", KeyPath = "lastName", Auto=false}
                    ]
    });

    dbStore.Stores.Add(new StoreSchema
    {
        Name = "Outbox",
        PrimaryKey = new IndexSpec { Auto = true }
    }
    );
});

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

await builder.Build().RunAsync();
