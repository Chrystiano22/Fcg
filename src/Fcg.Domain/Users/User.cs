using Fcg.Domain.Common;

namespace Fcg.Domain.Users;

public sealed class User
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public UserRole Role { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public bool IsAdministrator => Role == UserRole.Administrator;

    private User()
    {
    }

    private User(
        Guid id,
        string name,
        string email,
        string passwordHash,
        UserRole role,
        DateTime createdAt,
        DateTime updatedAt)
    {
        Id = id;
        Name = name;
        Email = email;
        PasswordHash = passwordHash;
        Role = role;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    public static User Register(
        string name,
        string email,
        string password,
        Func<string, string> hashPassword,
        UserRole role = UserRole.User)
    {
        if (hashPassword is null)
        {
            throw new ArgumentNullException(nameof(hashPassword));
        }

        var normalizedName = NormalizeName(name);
        var normalizedEmail = global::Fcg.Domain.Users.Email.Create(email);

        PasswordPolicy.Validate(password);

        var passwordHash = hashPassword(password);
        if (string.IsNullOrWhiteSpace(passwordHash))
        {
            throw new DomainValidationException("Password hash is required.");
        }

        var utcNow = DateTime.UtcNow;

        return new User(
            Guid.NewGuid(),
            normalizedName,
            normalizedEmail.Value,
            passwordHash,
            role,
            utcNow,
            utcNow);
    }

    public bool CanManagePlatform()
    {
        return IsAdministrator;
    }

    public void ChangeRole(UserRole role)
    {
        if (Role == role)
        {
            return;
        }

        Role = role;
        UpdatedAt = DateTime.UtcNow;
    }

    private static string NormalizeName(string name)
    {
        var normalized = name.Trim();

        if (string.IsNullOrWhiteSpace(normalized))
        {
            throw new DomainValidationException("Name is required.");
        }

        return normalized;
    }
}
