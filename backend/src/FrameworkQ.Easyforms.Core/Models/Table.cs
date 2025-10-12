namespace FrameworkQ.Easyforms.Core.Models;

/// <summary>
/// Row-based data entry table widget
/// </summary>
public class Table : Widget
{
    public string RowMode { get; set; } = "infinite";
    public int? MinRows { get; set; }
    public int? MaxRows { get; set; }
    public bool AllowAddRows { get; set; } = true;
    public bool AllowDeleteRows { get; set; } = true;
    public string[]? RowKey { get; set; }
    public List<Column> Columns { get; set; } = new();
    public List<Aggregate> Aggregates { get; set; } = new();
    public PrintConfig? PrintConfig { get; set; }

    public Table()
    {
        Type = WidgetType.Table;
    }
}
