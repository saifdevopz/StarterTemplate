using Common.Application.Database;
using Common.Domain.Errors;
using System.Domain.Features.Tenant;

namespace System.Application.Features.TenantType.CreateTenantType;

internal sealed class CreateTenantTypeHandler(IRepository<TenantTypeM> _repository)
        : ICommandHandler<CreateTenantTypeCommand, bool>
{
    public async Task<Result<bool>> Handle(CreateTenantTypeCommand request, CancellationToken cancellationToken)
    {
        if (request.Request.TenantTypeCode == "123")
        {
            return Result.Failure<bool>(CustomError.NotFound("404", "dd"));
        }

        TenantTypeM newTenantType = TenantTypeM.Create
        (
            request.Request.TenantTypeCode,
            request.Request.TenantTypeDesc
        );

        await _repository.AddAsync(newTenantType);
        await _repository.SaveChangesAsync(cancellationToken);

        return Result.Success(true);
    }
}
