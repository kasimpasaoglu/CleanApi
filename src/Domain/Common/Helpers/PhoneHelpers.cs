namespace Domain.Common.Helpers;

public static class PhoneHelpers
{
    private const int MinLength = 8;
    private const int MaxLength = 14;

    public static string NormalizePhoneNumber(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        var digits = new string(input.Where(char.IsDigit).ToArray());


        if (digits.StartsWith("00"))
            digits = digits[2..];


        if (digits.StartsWith("90") && digits.Length >= 12)
            digits = digits[2..];


        if (digits.Length == 11 && digits.StartsWith("0") && digits[1] is '2' or '3' or '4' or '5')
            digits = digits[1..];

        return digits;
    }
    
    
    public static bool NormalizeAndValidate(string? input, out string normalized)
    {
        normalized = NormalizePhoneNumber(input);
        return normalized.Length is >= MinLength and <= MaxLength;
    }

    /// <summary>
    /// Girdi normalize edildiğinde geçerli bir telefon (MinLength–MaxLength hane) mi? Validator'lar için.
    /// </summary>
    public static bool IsValid(string? input) => NormalizeAndValidate(input, out _);
}
