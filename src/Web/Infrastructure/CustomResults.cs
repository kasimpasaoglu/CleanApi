namespace Web.Infrastructure;

public static class CustomResults
{
    public static IResult Problem(Result result)
    {
        var pd = ProblemDetailsFactory.FromResult(result);

        return Results.Problem(
            title: pd.Title,
            detail: pd.Detail,
            type: pd.Type,
            statusCode: pd.Status,
            extensions: pd.Extensions.ToDictionary(k=>k.Key, v=> v.Value));
    }
}