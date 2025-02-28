using Common.Domain.TransferObjects.System;
using System.Application.Common.Interfaces;

namespace System.Application.Features.Authentication.RefreshToken;

internal sealed class RefreshTokenHandler(ITokenService _tokenService)
        : ICommandHandler<RefreshTokenCommand, TokenResponse>
{
    public async Task<Result<TokenResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        Result<TokenResponse> result = await _tokenService.RefreshToken(request.Request);
        return result;
    }
}