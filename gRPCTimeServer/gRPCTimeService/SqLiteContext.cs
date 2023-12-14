using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace gRPCTimeService;

using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;

/// <summary>
/// Represents the SQLite database context for timestamp entries.
/// </summary>
public class SqLiteContext : DbContext
{
    public DbSet<TimestampEntry> TimestampEntries { get; set; } = null!;

    /// <summary>
    /// Gets the path to the SQLite database file.
    /// </summary>
    public string DbPath { get; } = Path.Combine(AppContext.BaseDirectory, "sql.db");

    /// <summary>
    /// Configures the SQLite context with database options.
    /// </summary>
    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={DbPath}");
    
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder
            .Properties<DateTime>()
            .HaveConversion(typeof(UtcValueConverter));
    }

    private class UtcValueConverter : ValueConverter<DateTime, DateTime>
    {
        public UtcValueConverter()
            : base(v => v, v => DateTime.SpecifyKind(v, DateTimeKind.Utc))
        {
        }
    }
}

/// <summary>
/// Represents an entry in the SQLite database for timestamp information.
/// </summary>
public class TimestampEntry
{
    [Key]
    public int Id { get; set; }
    public DateTime Timestamp { get; set; }

    public TimestampEntry(DateTime timestamp)
    {
        Timestamp = timestamp;
    }
}
