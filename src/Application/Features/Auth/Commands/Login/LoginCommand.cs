using System.Security.Claims;

namespace Application.Features.Auth.Commands.Login;

public sealed record LoginCommand(string Email, string Password) : IRequest<Result<ClaimsPrincipal>>;
