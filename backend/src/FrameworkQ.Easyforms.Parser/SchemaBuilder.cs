namespace FrameworkQ.Easyforms.Parser;

using System.Text.Json;
using System.Text.Json.Serialization;
using FrameworkQ.Easyforms.Core.Interfaces;
using FrameworkQ.Easyforms.Core.Models;

/// <summary>
/// Builds canonical JSON schema from FormDefinition
/// </summary>
public class SchemaBuilder : ISchemaExtractor
{
    private readonly JsonSerializerOptions _jsonOptions;

    public SchemaBuilder()
    {
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
    }

    public Task<string> ExtractSchemaAsync(FormDefinition formDefinition)
    {
        var schema = new
        {
            form = new
            {
                id = formDefinition.Id,
                title = formDefinition.Title,
                version = formDefinition.Version,
                locales = formDefinition.Locales,
                storageMode = formDefinition.StorageMode,
                tags = formDefinition.Tags,
                pages = formDefinition.Pages.Select(page => new
                {
                    id = page.Id,
                    title = page.Title,
                    order = page.Order,
                    sections = page.Sections.Select(section => new
                    {
                        id = section.Id,
                        title = section.Title,
                        numberingScheme = section.NumberingScheme,
                        level = section.Level,
                        collapsible = section.Collapsible,
                        collapsed = section.Collapsed,
                        order = section.Order,
                        widgets = section.Widgets.Select(SerializeWidget)
                    })
                })
            }
        };

        var json = JsonSerializer.Serialize(schema, _jsonOptions);
        return Task.FromResult(json);
    }

    public Task<FormDefinition> ParseSchemaAsync(string schemaJson)
    {
        // Deserialize JSON back to FormDefinition
        var schema = JsonSerializer.Deserialize<JsonElement>(schemaJson);
        var formElement = schema.GetProperty("form");

        var formDef = new FormDefinition
        {
            Id = formElement.GetProperty("id").GetString() ?? string.Empty,
            Title = formElement.GetProperty("title").GetString() ?? string.Empty,
            Version = formElement.GetProperty("version").GetString() ?? "1.0",
            Locales = formElement.GetProperty("locales").EnumerateArray().Select(e => e.GetString() ?? string.Empty).ToArray(),
            StorageMode = formElement.GetProperty("storageMode").GetString() ?? "jsonb",
            Tags = formElement.GetProperty("tags").EnumerateArray().Select(e => e.GetString() ?? string.Empty).ToArray(),
            SchemaJson = schemaJson
        };

        // TODO: Deserialize pages, sections, widgets
        // This is needed for schema comparison and migration

        return Task.FromResult(formDef);
    }

    public Task<SchemaDiff> CompareAsync(string oldSchema, string newSchema)
    {
        // TODO: Implement schema comparison for US4 (migration)
        var diff = new SchemaDiff();
        return Task.FromResult(diff);
    }

    private object SerializeWidget(Widget widget)
    {
        return widget switch
        {
            Field field => new
            {
                type = "field",
                id = field.Id,
                field = new
                {
                    name = field.Name,
                    type = field.DataType,
                    label = field.Label,
                    required = field.Required,
                    @readonly = field.Readonly,
                    defaultValue = field.DefaultValue,
                    unit = field.Unit,
                    pattern = field.Pattern,
                    min = field.Min,
                    max = field.Max,
                    format = field.Format,
                    enumValues = field.EnumValues,
                    compute = field.Compute,
                    @override = field.Override,
                    when = field.When,
                    validationRules = field.ValidationRules,
                    fetchConfig = field.FetchConfig
                }
            },
            Group group => new
            {
                type = "group",
                id = group.Id,
                group = new
                {
                    layout = group.Layout,
                    when = group.When,
                    fields = group.Fields.Select(f => SerializeWidget(f))
                }
            },
            Table table => new
            {
                type = "table",
                id = table.Id,
                table = new
                {
                    rowMode = table.RowMode,
                    minRows = table.MinRows,
                    maxRows = table.MaxRows,
                    allowAddRows = table.AllowAddRows,
                    allowDeleteRows = table.AllowDeleteRows,
                    rowKey = table.RowKey,
                    columns = table.Columns,
                    aggregates = table.Aggregates,
                    printConfig = table.PrintConfig,
                    when = table.When
                }
            },
            Grid grid => new
            {
                type = "grid",
                id = grid.Id,
                grid = new
                {
                    rowGeneration = grid.RowGeneration,
                    columnGeneration = grid.ColumnGeneration,
                    cellType = grid.CellType,
                    cellEnumValues = grid.CellEnumValues,
                    when = grid.When
                }
            },
            CompositeWidget comp => new
            {
                type = "composite",
                id = comp.Id,
                composite = new
                {
                    name = comp.Name,
                    properties = comp.Properties,
                    isContainer = comp.IsContainer,
                    when = comp.When,
                    // children serialized if parsed
                    children = comp.Children.Select(SerializeWidget)
                }
            },
            _ => new { type = widget.Type.ToString().ToLower(), id = widget.Id }
        };
    }
}
