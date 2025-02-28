using Common.Domain.TransferObjects.System;

namespace System.Application.Features.Users.GetAllUser;

public sealed record GetAllUserQuery : IQuery<List<ReadUserDto>>;