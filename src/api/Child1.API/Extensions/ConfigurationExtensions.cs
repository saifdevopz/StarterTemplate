namespace Child1.API.Extensions;

internal static class ConfigurationExtensions
{
    internal static void AddModuleConfiguration(this IConfigurationBuilder configurationBuilder, string[] modules)
    {
        foreach (string module in modules)
        {
            configurationBuilder.AddJsonFile($"Configurations/{module}.config.Development.json", false, true);
            configurationBuilder.AddJsonFile($"Configurations/{module}.config.json", true, true);
        }
    }
}