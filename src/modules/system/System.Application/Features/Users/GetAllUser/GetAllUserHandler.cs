using Common.Application.Database;
using Common.Domain.TransferObjects.System;
using System.Domain.Identity;

namespace System.Application.Features.Users.GetAllUser;

internal sealed class GetAllUserHandler(IRepository<UserM> genericRepository)
    : IQueryHandler<GetAllUserQuery, List<ReadUserDto>>
{
    public async Task<Result<List<ReadUserDto>>> Handle(
        GetAllUserQuery request,
        CancellationToken cancellationToken)
    {
        List<UserM> objects = await genericRepository.GetAllAsync();

        List<ReadUserDto> result = objects.Select(obj => new ReadUserDto
        (
            obj.UserId,
            obj.Email,
            obj.IsActive
        )).ToList();

        return result;
    }
}