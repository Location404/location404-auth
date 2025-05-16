using System.Text.RegularExpressions;

namespace UserIdentity.Domain.ValueObject;

public sealed partial class EmailAddress : Common.ValueObject
{
    public EmailAddress(string emailAddress)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(emailAddress);
        if (!EmailAddressPattern().IsMatch(emailAddress))
        {
            throw new ArgumentException(nameof(EmailAddress), "Invalid email address format.");
        }

        Value = emailAddress;
    }

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase | RegexOptions.Compiled, "")]
    private static partial Regex EmailAddressPattern();
}
