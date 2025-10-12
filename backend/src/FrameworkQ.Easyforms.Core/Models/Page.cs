namespace FrameworkQ.Easyforms.Core.Models;

/// <summary>
/// Represents a logical page within a form
/// </summary>
public class Page
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public int Order { get; set; }
    public List<Section> Sections { get; set; } = new();
}
