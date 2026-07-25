using System.Security.Claims;

namespace Application.Features.Auth.Queries.FindMe;

public record FindMeQuery(ClaimsPrincipal User) : IRequest<Result<UserDto>>;