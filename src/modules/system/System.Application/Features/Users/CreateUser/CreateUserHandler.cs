using Common.Application.Database;
using Common.Domain.Errors;
using System.Domain.Identity;

namespace System.Application.Features.Users.CreateUser;

internal sealed class CreateUserHandler(IRepository<UserM> _userRepository)
        : ICommandHandler<CreateUserCommand, int>
{
    public async Task<Result<int>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        UserM? user = _userRepository.FindOne(u => u.Email == request.Request.Email, cancellationToken: cancellationToken);

        if (user is not null)
        {
            return Result.Failure<int>(CustomError.Conflict("406", "Email already exists."));
        }

        UserM newUser = UserM.Create(
            request.Request.Email,
            request.Request.Password);

        await _userRepository.AddAsync(newUser);
        await _userRepository.SaveChangesAsync(cancellationToken);

        return Result.Success(newUser.UserId);
    }
}
