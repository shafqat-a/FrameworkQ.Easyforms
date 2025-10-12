namespace FrameworkQ.Easyforms.Core.Interfaces;

using FrameworkQ.Easyforms.Core.Models;

/// <summary>
/// Interface for database-specific operations (SQL Server, PostgreSQL)
/// </summary>
public interface IDatabaseProvider
{
    /// <summary>
    /// Get provider name (sqlserver, postgresql)
    /// </summary>
    string ProviderName { get; }

    /// <summary>
    /// Map DSL data type to database-specific type
    /// </summary>
    /// <param name="dslType">DSL type (string, integer, decimal, etc.)</param>
    /// <returns>Database-specific type</returns>
    string MapDataType(string dslType);

    /// <summary>
    /// Generate CREATE TABLE DDL statement
    /// </summary>
    /// <param name="tableName">Table name</param>
    /// <param name="columns">Column definitions</param>
    /// <param name="constraints">Constraints (NOT NULL, CHECK, etc.)</param>
    /// <returns>DDL statement</returns>
    string GenerateCreateTableDdl(string tableName, List<ColumnDefinition> columns, List<string> constraints);

    /// <summary>
    /// Generate computed column syntax
    /// </summary>
    /// <param name="columnName">Column name</param>
    /// <param name="expression">SQL expression</param>
    /// <returns>Computed column DDL</returns>
    string GenerateComputedColumnDdl(string columnName, string expression);

    /// <summary>
    /// Execute DDL statement
    /// </summary>
    /// <param name="ddl">DDL statement</param>
    /// <param name="connectionString">Connection string</param>
    /// <returns>True if successful</returns>
    Task<bool> ExecuteDdlAsync(string ddl, string connectionString);

    /// <summary>
    /// Migrate schema from old to new version
    /// </summary>
    /// <param name="oldSchema">Old schema JSON</param>
    /// <param name="newSchema">New schema JSON</param>
    /// <param name="connectionString">Connection string</param>
    /// <returns>Migration result</returns>
    Task<MigrationResult> MigrateSchemaAsync(string oldSchema, string newSchema, string connectionString);
}

public class ColumnDefinition
{
    public string Name { get; set; } = string.Empty;
    public string DataType { get; set; } = string.Empty;
    public bool IsNullable { get; set; } = true;
    public bool IsComputed { get; set; }
    public string? ComputedExpression { get; set; }
    public string? DefaultValue { get; set; }
}

public class MigrationResult
{
    public bool Success { get; set; }
    public List<string> ExecutedSql { get; set; } = new();
    public List<string> Errors { get; set; } = new();
}
