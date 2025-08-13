using System.Text.RegularExpressions;

namespace UserIdentityService.Domain.ValueObjects;

public sealed partial class EmailAddress
{
    private static readonly Regex EmailRegex = EmailAddress.Regex();

    public string Value { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="EmailAddress"/> with the specified email value.
    /// </summary>
    /// <param name="value">The validated, normalized email string to store as the value.</param>
    private EmailAddress(string value)
    {
        Value = value;
    }

    /// <summary>
    /// Creates a validated EmailAddress value object from the provided email string.
    /// </summary>
    /// <param name="email">The email to validate and normalize (may include surrounding whitespace).</param>
    /// <returns>A new <see cref="EmailAddress"/> containing the normalized (trimmed, lowercased) email.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="email"/> is null, empty, whitespace, or does not match the required email format.</exception>
    public static EmailAddress Create(string email)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email, nameof(email));
        email = email.Trim();

        return !EmailRegex.IsMatch(email)
            ? throw new ArgumentException("Formato de email inválido.", nameof(email))
            : new EmailAddress(email.ToLowerInvariant());
    }

    /// <summary>
/// Returns the underlying email address string.
/// </summary>
/// <returns>The stored email address value.</returns>
public override string ToString() => Value;

    public static implicit operator string(EmailAddress e) => e.Value;


    /// <summary>
    /// Returns a compiled <see cref="Regex"/> instance for validating basic email addresses.
    /// The regex enforces a simple local@domain.tld structure and is created by the C# GeneratedRegex source generator.
    /// </summary>
    /// <returns>A compiled, case-insensitive <see cref="Regex"/> for the pattern <c>^[^@\s]+@[^@\s]+\.[^@\s]+$</c> using the "pt-BR" culture.</returns>
    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase | RegexOptions.Compiled, "pt-BR")]
    private static partial Regex Regex();
}
