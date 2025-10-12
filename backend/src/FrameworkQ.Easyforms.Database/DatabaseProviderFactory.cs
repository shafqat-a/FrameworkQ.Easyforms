namespace FrameworkQ.Easyforms.Database;

using FrameworkQ.Easyforms.Core.Interfaces;
using FrameworkQ.Easyforms.Database.Providers;

/// <summary>
/// Factory for creating database provider instances
/// </summary>
public class DatabaseProviderFactory
{
    /// <summary>
    /// Create a database provider based on provider name
    /// </summary>
    /// <param name="providerName">Provider name (sqlserver, postgresql)</param>
    /// <returns>Database provider instance</returns>
    /// <exception cref="ArgumentException">If provider name is invalid</exception>
    public static IDatabaseProvider Create(string providerName)
    {
        return providerName.ToLower() switch
        {
            "sqlserver" => new SqlServerProvider(),
            "postgresql" => new PostgreSqlProvider(),
            _ => throw new ArgumentException($"Unknown database provider: {providerName}", nameof(providerName))
        };
    }

    /// <summary>
    /// Get list of supported providers
    /// </summary>
    /// <returns>Array of provider names</returns>
    public static string[] GetSupportedProviders()
    {
        return new[] { "sqlserver", "postgresql" };
    }
}
