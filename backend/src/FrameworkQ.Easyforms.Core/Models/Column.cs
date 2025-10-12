namespace FrameworkQ.Easyforms.Core.Models;

/// <summary>
/// Table column definition
/// </summary>
public class Column
{
    public string Name { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string DataType { get; set; } = "string";
    public bool Required { get; set; }
    public bool Readonly { get; set; }
    public string? Unit { get; set; }
    public string? Format { get; set; }
    public string[]? EnumValues { get; set; }
    public string? Formula { get; set; }
    public string? DefaultValue { get; set; }
    public string? Min { get; set; }
    public string? Max { get; set; }
    public string? Pattern { get; set; }
    public string? Width { get; set; }
    public string? Align { get; set; }
    public string? VAlign { get; set; }
}
