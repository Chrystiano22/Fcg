using Fcg.Domain.Common;

namespace Fcg.Domain.Promotions;

public sealed class Promotion
{
    public Guid Id { get; private set; }
    public Guid GameId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public decimal DiscountPercentage { get; private set; }
    public DateTime StartsAt { get; private set; }
    public DateTime EndsAt { get; private set; }
    public bool Active { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private Promotion()
    {
    }

    private Promotion(
        Guid id,
        Guid gameId,
        string name,
        string description,
        decimal discountPercentage,
        DateTime startsAt,
        DateTime endsAt,
        bool active,
        DateTime createdAt,
        DateTime updatedAt)
    {
        Id = id;
        GameId = gameId;
        Name = name;
        Description = description;
        DiscountPercentage = discountPercentage;
        StartsAt = startsAt;
        EndsAt = endsAt;
        Active = active;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    public static Promotion Create(
        Guid gameId,
        string name,
        string description,
        decimal discountPercentage,
        DateTime startsAt,
        DateTime endsAt)
    {
        ValidateGameId(gameId);

        var normalizedName = NormalizeRequiredText(name, "Promotion name is required.");
        var normalizedDescription = NormalizeRequiredText(description, "Promotion description is required.");
        var normalizedStartsAt = NormalizeDate(startsAt, "Promotion start date is required.");
        var normalizedEndsAt = NormalizeDate(endsAt, "Promotion end date is required.");

        ValidateDiscountPercentage(discountPercentage);
        ValidateDateRange(normalizedStartsAt, normalizedEndsAt);

        var utcNow = DateTime.UtcNow;

        return new Promotion(
            Guid.NewGuid(),
            gameId,
            normalizedName,
            normalizedDescription,
            discountPercentage,
            normalizedStartsAt,
            normalizedEndsAt,
            active: true,
            utcNow,
            utcNow);
    }

    public bool IsCurrentlyActive(DateTime utcNow)
    {
        return Active && utcNow >= StartsAt && utcNow <= EndsAt;
    }

    private static void ValidateGameId(Guid gameId)
    {
        if (gameId == Guid.Empty)
        {
            throw new DomainValidationException("Game id is required.");
        }
    }

    private static void ValidateDiscountPercentage(decimal discountPercentage)
    {
        if (discountPercentage <= 0 || discountPercentage > 100)
        {
            throw new DomainValidationException("Discount percentage must be greater than 0 and less than or equal to 100.");
        }
    }

    private static void ValidateDateRange(DateTime startsAt, DateTime endsAt)
    {
        if (endsAt <= startsAt)
        {
            throw new DomainValidationException("Promotion end date must be greater than the start date.");
        }
    }

    private static string NormalizeRequiredText(string value, string errorMessage)
    {
        var normalized = value?.Trim();

        if (string.IsNullOrWhiteSpace(normalized))
        {
            throw new DomainValidationException(errorMessage);
        }

        return normalized;
    }

    private static DateTime NormalizeDate(DateTime value, string errorMessage)
    {
        if (value == default)
        {
            throw new DomainValidationException(errorMessage);
        }

        return value.Kind switch
        {
            DateTimeKind.Utc => value,
            DateTimeKind.Local => value.ToUniversalTime(),
            _ => DateTime.SpecifyKind(value, DateTimeKind.Utc)
        };
    }
}
