namespace System.Application.Features.Users.DeleteUser;

public sealed record DeleteUserCommand(int Request)
    : ICommand<bool>;