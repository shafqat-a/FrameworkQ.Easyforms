namespace FrameworkQ.Easyforms.Core.Models;

/// <summary>
/// Base class for all interactive form elements
/// </summary>
public abstract class Widget
{
    public string Id { get; set; } = string.Empty;
    public WidgetType Type { get; set; }
    public string? When { get; set; }
}

public enum WidgetType
{
    Field,
    Group,
    Table,
    Grid,
    Composite,
    Checklist,
    Signature,
    FormHeader,
    Notes,
    RadioGroup,
    CheckboxGroup,
    HierarchicalChecklist
}
