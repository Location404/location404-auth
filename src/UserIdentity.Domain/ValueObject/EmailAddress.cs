using System.Text.RegularExpressions;

namespace UserIdentity.Domain.ValueObject;

public sealed partial class Email : Common.ValueObject
{
    public Email(string email)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        if (!EmailPattern().IsMatch(email))
        {
            throw new ArgumentException(nameof(Email), "Invalid email address format.");
        }

        Value = email;
    }

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase | RegexOptions.Compiled, "")]
    private static partial Regex EmailPattern();
}
