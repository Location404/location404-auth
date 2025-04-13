namespace UserIdentity.Domain.Validations;

public static class ArgumentValidationExtensions
{
    /// <summary>
    /// Throws an ArgumentException if the string contains invalid characters.
    /// Only the specified characters are allowed.
    /// </summary>
    /// <param name="value">The string to validate.</param>
    /// <param name="paramName">The name of the parameter being validated, used in the exception message.</param>
    /// <param name="allowedCharacters">An array of characters that are allowed in the string.</param>
    public static void ThrowIfContainsInvalidCharacters(string value, string paramName, params char[] allowedCharacters)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value, paramName);

        var allowedSet = new HashSet<char>(allowedCharacters);
        Func<char, bool> isValidChar = c => char.IsLetterOrDigit(c) || allowedSet.Contains(c);

        if (!value.All(isValidChar))
        {
            string allowedCharsString = new(allowedCharacters);
            string message = allowedCharacters.Length > 0
                ? $"Value contains invalid characters. Only letters, digits, and the following characters are allowed: {allowedCharsString}."
                : "Value contains invalid characters. Only letters and digits are allowed.";

            throw new ArgumentException(message, paramName);
        }
    }
}
