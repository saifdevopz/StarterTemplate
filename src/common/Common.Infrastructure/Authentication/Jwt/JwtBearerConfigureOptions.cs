using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Common.Infrastructure.Authentication.Jwt;

internal sealed class JwtBearerConfigureOptions(IConfiguration configuration)
        : IConfigureNamedOptions<JwtBearerOptions>
{
    private const string ConfigurationSectionName = "Authentication";

    public void Configure(JwtBearerOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        IConfigurationSection section = configuration.GetSection(ConfigurationSectionName);

        if (!section.Exists())
        {
            throw new InvalidOperationException($"Configuration section '{ConfigurationSectionName}' not found.");
        }

        section.Bind(options);
    }

    public void Configure(string? name, JwtBearerOptions options)
    {
        Configure(options);
    }
}
