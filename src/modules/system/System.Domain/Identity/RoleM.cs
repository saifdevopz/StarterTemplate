namespace System.Domain.Identity;

public sealed class RoleM
{
    public int RoleId { get; }
    public string RoleName { get; private set; } = string.Empty;
    public string NormalizedRoleName { get; private set; } = string.Empty;

    public static RoleM Create(string roleName)
    {
        ArgumentNullException.ThrowIfNull(roleName);

        RoleM obj = new()
        {
            RoleName = roleName,
            NormalizedRoleName = roleName.ToUpperInvariant()
        };

        return obj;
    }
}
