namespace Application.Features.Auth.Queries.FindMe;

public class FindMeHandler(IIdentityService identityService) : IRequestHandler<FindMeQuery, Result<UserDto>>
{
    public async Task<Result<UserDto>> Handle(FindMeQuery request, CancellationToken cancellationToken)
    {
        var userId = request.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId)) return Result.Failure<UserDto>(Error.NotFound(ErrorCodes.NotFound.FindMe, $"Kullanıcı doğrulanamadı.: {userId}"));

        var userInfo = await identityService.GetUserInfoByIdAsync(userId);

        return userInfo is null
            ? Result.Failure<UserDto>(Error.NotFound(ErrorCodes.NotFound.User, $"Kullanıcı bulunamadı: {userId}"))
            : Result.Success(userInfo);
    }
}