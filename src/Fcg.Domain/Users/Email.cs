using System.Text.RegularExpressions;
using Fcg.Domain.Common;

namespace Fcg.Domain.Users;

public sealed partial record Email
{
    public string Value { get; }

    private Email(string value)
    {
        Value = value;
    }

    public static Email Create(string value)
    {
        var normalized = value.Trim().ToLowerInvariant();

        if (string.IsNullOrWhiteSpace(normalized))
        {
            throw new DomainValidationException("Email is required.");
        }

        if (!EmailRegex().IsMatch(normalized))
        {
            throw new DomainValidationException("Email format is invalid.");
        }

        return new Email(normalized);
    }

    public override string ToString()
    {
        return Value;
    }

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
    private static partial Regex EmailRegex();
}
