namespace FrameworkQ.Easyforms.Core.Interfaces;

using FrameworkQ.Easyforms.Core.Models;

/// <summary>
/// Interface for extracting JSON schema from FormDefinition
/// </summary>
public interface ISchemaExtractor
{
    /// <summary>
    /// Extract canonical JSON schema from FormDefinition
    /// </summary>
    /// <param name="formDefinition">Form definition model</param>
    /// <returns>JSON schema string</returns>
    Task<string> ExtractSchemaAsync(FormDefinition formDefinition);

    /// <summary>
    /// Parse JSON schema back into FormDefinition
    /// </summary>
    /// <param name="schemaJson">JSON schema string</param>
    /// <returns>Form definition model</returns>
    Task<FormDefinition> ParseSchemaAsync(string schemaJson);

    /// <summary>
    /// Compare two schemas and identify differences
    /// </summary>
    /// <param name="oldSchema">Old schema JSON</param>
    /// <param name="newSchema">New schema JSON</param>
    /// <returns>Schema differences</returns>
    Task<SchemaDiff> CompareAsync(string oldSchema, string newSchema);
}

public class SchemaDiff
{
    public List<string> AddedFields { get; set; } = new();
    public List<string> RemovedFields { get; set; } = new();
    public List<string> ModifiedFields { get; set; } = new();
    public List<string> RenamedFields { get; set; } = new();
    public bool HasBreakingChanges { get; set; }
}
