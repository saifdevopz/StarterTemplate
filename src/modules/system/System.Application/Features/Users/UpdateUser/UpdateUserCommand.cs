using Common.Domain.TransferObjects.System;

namespace System.Application.Features.Users.UpdateUser;

public sealed record UpdateUserCommand(WriteUserDto Request)
    : ICommand<bool>;