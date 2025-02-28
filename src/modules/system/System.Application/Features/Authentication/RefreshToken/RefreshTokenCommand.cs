using Common.Domain.TransferObjects.System;

namespace System.Application.Features.Authentication.RefreshToken;

public sealed record RefreshTokenCommand(RefreshTokenRequest Request)
    : ICommand<TokenResponse>;