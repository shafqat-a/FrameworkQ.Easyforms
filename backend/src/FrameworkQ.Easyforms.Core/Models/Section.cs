namespace FrameworkQ.Easyforms.Core.Models;

/// <summary>
/// Represents a grouping of widgets within a page
/// </summary>
public class Section
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string NumberingScheme { get; set; } = "none";
    public int Level { get; set; }
    public bool Collapsible { get; set; }
    public bool Collapsed { get; set; }
    public int Order { get; set; }
    public List<Widget> Widgets { get; set; } = new();
}
