namespace System.Domain.Features.Identity;

public static class IdentityErrors
{
    public static CustomError NotFound(int userId)
    {
        return CustomError.NotFound("Users.NotFound", $"The user with the identifier {userId} not found");
    }

    public static CustomError NotFound(string identityId)
    {
        return CustomError.NotFound("Users.NotFound", $"The user with the IDP identifier {identityId} not found");
    }

    public static CustomError Conflict(int identityId)
    {
        return CustomError.Conflict("Users.Conflict", $"The user with the identifier {identityId} already exists.");
    }

    public static CustomError Conflict(string email)
    {
        return CustomError.Conflict("Users.Conflict", $"The email {email} already exists.");
    }
}
