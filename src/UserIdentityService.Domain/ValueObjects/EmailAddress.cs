using System.Text.RegularExpressions;

namespace UserIdentityService.Domain.ValueObjects;

public sealed partial class EmailAddress
{
    private static readonly Regex EmailRegex = EmailAddress.Regex();

    public string Value { get; }

    private EmailAddress(string value)
    {
        Value = value;
    }

    public static EmailAddress Create(string email)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email, nameof(email));
        email = email.Trim();

        return !EmailRegex.IsMatch(email)
            ? throw new ArgumentException("Formato de email inválido.", nameof(email))
            : new EmailAddress(email.ToLowerInvariant());
    }

    public override string ToString() => Value;

    public static implicit operator string(EmailAddress e) => e.Value;


    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase | RegexOptions.Compiled, "pt-BR")]
    private static partial Regex Regex();
}
