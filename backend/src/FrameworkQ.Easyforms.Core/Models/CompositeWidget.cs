namespace FrameworkQ.Easyforms.Core.Models;

/// <summary>
/// Composite widget (custom control) with properties and optional children
/// </summary>
public class CompositeWidget : Widget
{
    public string Name { get; set; } = string.Empty; // component name/type e.g., "meter-block"
    public Dictionary<string, string> Properties { get; set; } = new(); // data-prop-* collected values
    public bool IsContainer { get; set; } // true if widget can contain children
    public List<Widget> Children { get; set; } = new(); // parsed child widgets if container

    public CompositeWidget()
    {
        Type = WidgetType.Composite;
    }
}

