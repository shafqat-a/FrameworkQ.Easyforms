namespace FrameworkQ.Easyforms.Parser.WidgetParsers;

using AngleSharp.Dom;
using FrameworkQ.Easyforms.Core.Models;

/// <summary>
/// Parser for Grid widgets (data-grid attribute)
/// </summary>
public class GridParser
{
    public Grid? Parse(IElement element)
    {
        if (!element.HasAttribute("data-grid"))
        {
            return null;
        }

        var grid = new Grid
        {
            Id = element.GetAttribute("id") ?? $"grid-{Guid.NewGuid().ToString("N")[..8]}",
            RowGeneration = GetAttributeValue(element, "data-row-gen") ?? GetAttributeValue(element, "data-rows") ?? "finite",
            ColumnGeneration = GetAttributeValue(element, "data-col-gen") ?? GetAttributeValue(element, "data-columns"),
            CellType = GetAttributeValue(element, "data-cell-type") ?? "string",
            CellEnumValues = GetAttributeValue(element, "data-cell-enum")?.Split('|', StringSplitOptions.RemoveEmptyEntries),
            When = GetAttributeValue(element, "data-when")
        };

        return grid;
    }

    private static string? GetAttributeValue(IElement element, string attributeName)
    {
        return element.GetAttribute(attributeName);
    }
}
