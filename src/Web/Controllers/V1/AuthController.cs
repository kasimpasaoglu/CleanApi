namespace Web.Controllers.V1;

[ApiController]
[Authorize]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class AuthController() : BaseApiController
{
    private const string SessionCookieName = "CleanApi_session";

    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    public async Task<IResult> Login([FromBody] LoginCommand command, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(command, cancellationToken);
        if (result.IsFailure)
        {
            return CustomResults.Problem(result);
        }

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, result.Value);

        var sessionToken = ExtractCookieValue(Response.Headers.SetCookie, SessionCookieName);
        if (string.IsNullOrWhiteSpace(sessionToken))
        {
            return Results.Problem(
                title: ErrorCodes.Problem,
                detail: "Oturum cookie'si oluşturulamadı.",
                statusCode: StatusCodes.Status500InternalServerError);
        }

        var meResult = await Mediator.Send(new FindMeQuery(result.Value), cancellationToken);
        return meResult.Match(
            user => Results.Ok(new LoginResponse(sessionToken, user)),
            CustomResults.Problem);
    }


    [HttpGet]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    public async Task<IResult> Get(CancellationToken cancellationToken)
    {
        var response = await Mediator.Send(new FindMeQuery(User), cancellationToken);
        return response.Match(Results.Ok, CustomResults.Problem);
    }

    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Results.Ok();
    }


    private static string? ExtractCookieValue(StringValues setCookieHeaders, string cookieName)
    {
        foreach (var setCookieHeader in setCookieHeaders)
        {
            if (string.IsNullOrEmpty(setCookieHeader))
            {
                continue;
            }

            if (!setCookieHeader.StartsWith($"{cookieName}=", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            var valueStartIndex = cookieName.Length + 1;
            var valueEndIndex = setCookieHeader.IndexOf(';', valueStartIndex);

            return valueEndIndex >= 0
                ? setCookieHeader[valueStartIndex..valueEndIndex]
                : setCookieHeader[valueStartIndex..];
        }

        return null;
    }

    private sealed record LoginResponse(string SessionToken, UserDto User);
}