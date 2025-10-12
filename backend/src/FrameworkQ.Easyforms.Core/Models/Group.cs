namespace FrameworkQ.Easyforms.Core.Models;

/// <summary>
/// Container for multiple fields with layout hints
/// </summary>
public class Group : Widget
{
    public string Layout { get; set; } = "columns:1";
    public List<Field> Fields { get; set; } = new();

    public Group()
    {
        Type = WidgetType.Group;
    }
}
