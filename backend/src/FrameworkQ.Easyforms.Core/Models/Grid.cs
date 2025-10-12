namespace FrameworkQ.Easyforms.Core.Models;

/// <summary>
/// 2D matrix with generated rows/columns
/// </summary>
public class Grid : Widget
{
    public string RowGeneration { get; set; } = "finite";
    public string? ColumnGeneration { get; set; }
    public string CellType { get; set; } = "string";
    public string[]? CellEnumValues { get; set; }

    public Grid()
    {
        Type = WidgetType.Grid;
    }
}
