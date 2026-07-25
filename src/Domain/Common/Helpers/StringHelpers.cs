namespace Domain.Common.Helpers;

public static class StringHelpers
{
    public static string CapitalizeEachWord(string input)
    {
        return string.Join(" ",
            input
                .Trim()
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(word => char.ToUpperInvariant(word[0]) + word[1..].ToLowerInvariant()));
    }
}