namespace System.Domain.Identity;

public sealed class PermissionM
{
    public int PermissionId { get; }
    public string PermissionCode { get; private set; } = string.Empty;

    public static PermissionM Create(string permissionCode)
    {
        PermissionM obj = new()
        {
            PermissionCode = permissionCode,
        };

        return obj;
    }
}