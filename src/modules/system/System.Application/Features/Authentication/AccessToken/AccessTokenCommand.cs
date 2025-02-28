using Common.Domain.TransferObjects.System;

namespace System.Application.Features.Authentication.AccessToken;

public sealed record AccessTokenCommand(AccessTokenRequest Request)
    : ICommand<TokenResponse>;
