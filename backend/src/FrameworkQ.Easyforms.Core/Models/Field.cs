namespace FrameworkQ.Easyforms.Core.Models;

/// <summary>
/// Single input field widget
/// </summary>
public class Field : Widget
{
    public string Name { get; set; } = string.Empty;
    public string DataType { get; set; } = "string";
    public string? Label { get; set; }
    public bool Required { get; set; }
    public bool Readonly { get; set; }
    public string? DefaultValue { get; set; }
    public string? Unit { get; set; }
    public string? Pattern { get; set; }
    public string? Min { get; set; }
    public string? Max { get; set; }
    public string? Format { get; set; }
    public string[]? EnumValues { get; set; }
    public string? Compute { get; set; }
    public bool Override { get; set; }
    public List<ValidationRule> ValidationRules { get; set; } = new();
    public FetchConfig? FetchConfig { get; set; }

    public Field()
    {
        Type = WidgetType.Field;
    }
}
