namespace FrameworkQ.Easyforms.Core.Models;

/// <summary>
/// Represents a complete form template
/// </summary>
public class FormDefinition
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Version { get; set; } = "1.0";
    public string[] Locales { get; set; } = Array.Empty<string>();
    public string StorageMode { get; set; } = "jsonb";
    public string[] Tags { get; set; } = Array.Empty<string>();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public string HtmlSource { get; set; } = string.Empty;
    public string SchemaJson { get; set; } = string.Empty;
    public List<Page> Pages { get; set; } = new();
}
