using System.Domain.Features.Identity;

namespace System.Application.Features.Users.CreateUser;

public sealed record CreateUserCommand(CreateUserDto Request) : ICommand<int>;
