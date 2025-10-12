namespace FrameworkQ.Easyforms.Database.Providers;

using FrameworkQ.Easyforms.Core.Interfaces;
using Npgsql;

/// <summary>
/// PostgreSQL database provider implementation
/// </summary>
public class PostgreSqlProvider : IDatabaseProvider
{
    public string ProviderName => "postgresql";

    public string MapDataType(string dslType)
    {
        return dslType.ToLower() switch
        {
            "string" => "VARCHAR(255)",
            "text" => "TEXT",
            "integer" => "INTEGER",
            "decimal" => "NUMERIC(18,6)",
            "date" => "DATE",
            "time" => "TIME",
            "datetime" => "TIMESTAMPTZ",
            "bool" => "BOOLEAN",
            "enum" => "VARCHAR(100)",
            "attachment" => "VARCHAR(500)",
            "signature" => "VARCHAR(500)",
            _ => "VARCHAR(255)"
        };
    }

    public string GenerateCreateTableDdl(string tableName, List<ColumnDefinition> columns, List<string> constraints)
    {
        var columnDefs = columns.Select(col =>
        {
            var parts = new List<string> { $"\"{col.Name}\"", col.DataType };

            if (!col.IsNullable) parts.Add("NOT NULL");
            if (col.DefaultValue != null) parts.Add($"DEFAULT {col.DefaultValue}");

            return string.Join(" ", parts);
        });

        var allDefs = columnDefs.Concat(constraints);
        return $"CREATE TABLE \"{tableName}\" ({string.Join(", ", allDefs)});";
    }

    public string GenerateComputedColumnDdl(string columnName, string expression)
    {
        return $"\"{columnName}\" GENERATED ALWAYS AS ({expression}) STORED";
    }

    public async Task<bool> ExecuteDdlAsync(string ddl, string connectionString)
    {
        await using var connection = new NpgsqlConnection(connectionString);
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
