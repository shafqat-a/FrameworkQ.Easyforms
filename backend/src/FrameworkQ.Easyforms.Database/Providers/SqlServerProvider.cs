namespace FrameworkQ.Easyforms.Database.Providers;

using FrameworkQ.Easyforms.Core.Interfaces;
using Microsoft.Data.SqlClient;

/// <summary>
/// SQL Server database provider implementation
/// </summary>
public class SqlServerProvider : IDatabaseProvider
{
    public string ProviderName => "sqlserver";

    public string MapDataType(string dslType)
    {
        return dslType.ToLower() switch
        {
            "string" => "NVARCHAR(255)",
            "text" => "NVARCHAR(MAX)",
            "integer" => "INT",
            "decimal" => "DECIMAL(18,6)",
            "date" => "DATE",
            "time" => "TIME",
            "datetime" => "DATETIME2",
            "bool" => "BIT",
            "enum" => "NVARCHAR(100)",
            "attachment" => "NVARCHAR(500)",
            "signature" => "NVARCHAR(500)",
            _ => "NVARCHAR(255)"
        };
    }

    public string GenerateCreateTableDdl(string tableName, List<ColumnDefinition> columns, List<string> constraints)
    {
        var columnDefs = columns.Select(col =>
        {
            var parts = new List<string> { $"[{col.Name}]", col.DataType };

            if (!col.IsNullable) parts.Add("NOT NULL");
            if (col.DefaultValue != null) parts.Add($"DEFAULT {col.DefaultValue}");

            return string.Join(" ", parts);
        });

        var allDefs = columnDefs.Concat(constraints);
        return $"CREATE TABLE [{tableName}] ({string.Join(", ", allDefs)});";
    }

    public string GenerateComputedColumnDdl(string columnName, string expression)
    {
        return $"[{columnName}] AS ({expression}) PERSISTED";
    }

    public async Task<bool> ExecuteDdlAsync(string ddl, string connectionString)
    {
        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();

        await using var command = connection.CreateCommand();
        command.CommandText = ddl;
        await command.ExecuteNonQueryAsync();

        return true;
    }

    public Task<MigrationResult> MigrateSchemaAsync(string oldSchema, string newSchema, string connectionString)
    {
        // TODO: Implement in US4 (Schema Extraction and Database Generation)
        throw new NotImplementedException("Schema migration will be implemented in US4");
    }
}
