using BlazorTemplate.Client.Registrars;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace BlazorTemplate.Client.Extensions;

internal static class RegistrarExtensions
{
    public static void RegisterServices(this WebAssemblyHostBuilder builder, Type scanningType)
    {
        try
        {
            IEnumerable<IWebAssemblyHostBuilderRegistrar> registrars = GetRegistrars<IWebAssemblyHostBuilderRegistrar>(scanningType);
            foreach (IWebAssemblyHostBuilderRegistrar registrar in registrars)
            {
                registrar.RegisterServices(builder);
            }
        }
        catch (Exception ex)
        {

            Console.WriteLine($"An error occurred registering services: {ex.Message}");
            throw;
        }
    }


    private static IEnumerable<T> GetRegistrars<T>(Type scanningType) where T : IRegistrar
    {
        return scanningType.Assembly.GetTypes()
            .Where(t => t.IsAssignableTo(typeof(T)) && !t.IsAbstract && !t.IsInterface)
            .Select(Activator.CreateInstance)
            .Cast<T>();
    }

}