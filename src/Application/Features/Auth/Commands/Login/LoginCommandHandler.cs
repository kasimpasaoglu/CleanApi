namespace Application.Features.Auth.Commands.Login;

public sealed class LoginCommandHandler(IIdentityService identityService)
    : IRequestHandler<LoginCommand, Result<ClaimsPrincipal>>
{
    public Task<Result<ClaimsPrincipal>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        return identityService.AuthenticateAsync(request.Email, request.Password);
    }
}