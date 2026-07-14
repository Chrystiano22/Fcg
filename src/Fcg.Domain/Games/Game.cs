using Fcg.Domain.Common;

namespace Fcg.Domain.Games;

public sealed class Game
{
    public Guid Id { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public decimal Price { get; private set; }
    public string Category { get; private set; } = string.Empty;
    public bool Active { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private Game()
    {
    }

    private Game(
        Guid id,
        string title,
        string description,
        decimal price,
        string category,
        bool active,
        DateTime createdAt,
        DateTime updatedAt)
    {
        Id = id;
        Title = title;
        Description = description;
        Price = price;
        Category = category;
        Active = active;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    public static Game Create(
        string title,
        string description,
        decimal price,
        string category)
    {
        var normalizedTitle = NormalizeRequiredText(title, "Title is required.");
        var normalizedDescription = NormalizeRequiredText(description, "Description is required.");
        var normalizedCategory = NormalizeRequiredText(category, "Category is required.");

        ValidatePrice(price);

        var utcNow = DateTime.UtcNow;

        return new Game(
            Guid.NewGuid(),
            normalizedTitle,
            normalizedDescription,
            price,
            normalizedCategory,
            active: true,
            utcNow,
            utcNow);
    }

    public void UpdateDetails(
        string title,
        string description,
        decimal price,
        string category)
    {
        Title = NormalizeRequiredText(title, "Title is required.");
        Description = NormalizeRequiredText(description, "Description is required.");
        Category = NormalizeRequiredText(category, "Category is required.");

        ValidatePrice(price);

        Price = price;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        if (Active)
        {
            return;
        }

        Active = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        if (!Active)
        {
            return;
        }

        Active = false;
        UpdatedAt = DateTime.UtcNow;
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

    private static void ValidatePrice(decimal price)
    {
        if (price < 0)
        {
            throw new DomainValidationException("Price cannot be negative.");
        }
    }
}
