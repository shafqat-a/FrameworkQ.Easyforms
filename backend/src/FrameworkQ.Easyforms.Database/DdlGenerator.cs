namespace FrameworkQ.Easyforms.Database;

using FrameworkQ.Easyforms.Core.Interfaces;
using FrameworkQ.Easyforms.Core.Models;
using FrameworkQ.Easyforms.Core.Expressions;
using System.Text;

/// <summary>
/// Generates SQL DDL statements for form schemas
/// </summary>
public class DdlGenerator
{
    private readonly IDatabaseProvider _provider;

    public DdlGenerator(IDatabaseProvider provider)
    {
        _provider = provider;
    }

    /// <summary>
    /// Generate DDL for all reporting tables in a form
    /// </summary>
    public List<string> GenerateFormDdl(FormDefinition formDefinition)
    {
        var ddlStatements = new List<string>();

        // Generate reporting tables for each table/grid widget
        foreach (var page in formDefinition.Pages)
        {
            foreach (var section in page.Sections)
            {
                foreach (var widget in section.Widgets)
                {
                    if (widget is Table table)
                    {
                        var tableDdl = GenerateReportingTableDdl(formDefinition.Id, page.Id, section.Id, table);
                        ddlStatements.Add(tableDdl);
                    }
                    else if (widget is Grid grid)
                    {
                        var gridDdl = GenerateGridTableDdl(formDefinition.Id, page.Id, section.Id, grid);
                        ddlStatements.Add(gridDdl);
                    }
                }
            }
        }

        return ddlStatements;
    }

    /// <summary>
    /// Generate reporting table DDL for a table widget
    /// </summary>
    private string GenerateReportingTableDdl(string formId, string pageId, string sectionId, Table table)
    {
        // Table name: formId_pageId_sectionId_widgetId
        var tableName = $"{formId}_{pageId}_{sectionId}_{table.Id}";
        tableName = SanitizeTableName(tableName);

        var columns = new List<ColumnDefinition>();

        // Standard columns
        columns.Add(new ColumnDefinition
        {
            Name = "instance_id",
            DataType = _provider.ProviderName == "sqlserver" ? "UNIQUEIDENTIFIER" : "UUID",
            IsNullable = false
        });

        columns.Add(new ColumnDefinition
        {
            Name = "page_id",
            DataType = _provider.MapDataType("string"),
            IsNullable = false,
            DefaultValue = $"'{pageId}'"
        });

        columns.Add(new ColumnDefinition
        {
            Name = "section_id",
            DataType = _provider.MapDataType("string"),
            IsNullable = false,
            DefaultValue = $"'{sectionId}'"
        });

        columns.Add(new ColumnDefinition
        {
            Name = "widget_id",
            DataType = _provider.MapDataType("string"),
            IsNullable = false,
            DefaultValue = $"'{table.Id}'"
        });

        columns.Add(new ColumnDefinition
        {
            Name = "row_index",
            DataType = _provider.MapDataType("integer"),
            IsNullable = false
        });

        columns.Add(new ColumnDefinition
        {
            Name = "recorded_at",
            DataType = _provider.ProviderName == "sqlserver" ? "DATETIME2" : "TIMESTAMPTZ",
            IsNullable = false,
            DefaultValue = _provider.ProviderName == "sqlserver" ? "GETUTCDATE()" : "NOW()"
        });

        // Data columns from table definition
        foreach (var column in table.Columns)
        {
            var colDef = new ColumnDefinition
            {
                Name = column.Name,
                DataType = _provider.MapDataType(column.DataType),
                IsNullable = !column.Required
            };

            // Computed columns
            if (!string.IsNullOrEmpty(column.Formula))
            {
                colDef.IsComputed = true;
                colDef.ComputedExpression = ConvertFormulaToSql(column.Formula);
            }

            columns.Add(colDef);
        }

        var constraints = new List<string>
        {
            "FOREIGN KEY (instance_id) REFERENCES form_instances(instance_id) ON DELETE CASCADE"
        };

        // Unique constraint from row key
        if (table.RowKey != null && table.RowKey.Length > 0)
        {
            var keyColumns = string.Join(", ", table.RowKey);
            constraints.Add($"UNIQUE (instance_id, {keyColumns})");
        }

        return _provider.GenerateCreateTableDdl(tableName, columns, constraints);
    }

    /// <summary>
    /// Generate grid table DDL
    /// </summary>
    private string GenerateGridTableDdl(string formId, string pageId, string sectionId, Grid grid)
    {
        // Similar to table but with generated rows/columns
        // Simplified for now
        var tableName = $"{formId}_{pageId}_{sectionId}_{grid.Id}";
        tableName = SanitizeTableName(tableName);

        var columns = new List<ColumnDefinition>
        {
            new() { Name = "instance_id", DataType = _provider.ProviderName == "sqlserver" ? "UNIQUEIDENTIFIER" : "UUID", IsNullable = false },
            new() { Name = "row_key", DataType = _provider.MapDataType("string"), IsNullable = false },
            new() { Name = "col_key", DataType = _provider.MapDataType("string"), IsNullable = false },
            new() { Name = "value", DataType = _provider.MapDataType(grid.CellType), IsNullable = true }
        };

        var constraints = new List<string>
        {
            "FOREIGN KEY (instance_id) REFERENCES form_instances(instance_id) ON DELETE CASCADE",
            "UNIQUE (instance_id, row_key, col_key)"
        };

        return _provider.GenerateCreateTableDdl(tableName, columns, constraints);
    }

    /// <summary>
    /// Convert DSL formula to SQL expression
    /// </summary>
    private string ConvertFormulaToSql(string formula)
    {
        try
        {
            return Evaluator.ToSql(formula);
        }
        catch
        {
            // If conversion fails, wrap in COALESCE for null safety
            return $"COALESCE({formula}, 0)";
        }
    }

    /// <summary>
    /// Sanitize table name (lowercase, replace hyphens with underscores)
    /// </summary>
    private string SanitizeTableName(string tableName)
    {
        return tableName.ToLower().Replace('-', '_');
    }
}
