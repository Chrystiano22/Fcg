using Fcg.Domain.Common;

namespace Fcg.Domain.Users;

internal static class PasswordPolicy
{
    public static void Validate(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            throw new DomainValidationException("Password is required.");
        }

        if (password.Length < 8)
        {
            throw new DomainValidationException("Password must have at least 8 characters.");
        }

        if (!password.Any(char.IsLetter))
        {
            throw new DomainValidationException("Password must contain at least one letter.");
        }

        if (!password.Any(char.IsDigit))
        {
            throw new DomainValidationException("Password must contain at least one number.");
        }

        if (!password.Any(IsSpecialCharacter))
        {
            throw new DomainValidationException("Password must contain at least one special character.");
        }
    }

    private static bool IsSpecialCharacter(char character)
    {
        return !char.IsLetterOrDigit(character) && !char.IsWhiteSpace(character);
    }
}
