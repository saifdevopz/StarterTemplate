using Common.Domain.TransferObjects.System;

namespace System.Application.Features.Users.GetByIdUser;

public sealed record GetByIdUserQuery(int Request) : IQuery<ReadUserDto>;