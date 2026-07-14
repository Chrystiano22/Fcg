using System.ComponentModel.DataAnnotations;

namespace Fcg.Api.Configuration;

public sealed class DatabaseOptions
{
    public const string SectionName = "Database";

    [Required]
    public string Provider { get; init; } = "Sqlite";

    [Required]
    public string ConnectionString { get; init; } = string.Empty;
}
