namespace FrameworkQ.Easyforms.Parser.WidgetParsers;

using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using FrameworkQ.Easyforms.Core.Models;

/// <summary>
/// Parser for Table widgets (data-table attribute)
/// </summary>
public class TableParser
{
    public Table? Parse(IElement element)
    {
        if (!element.HasAttribute("data-table"))
        {
            return null;
        }

        var table = new Table
        {
            Id = element.GetAttribute("id") ?? $"table-{Guid.NewGuid().ToString("N")[..8]}",
            RowMode = GetAttributeValue(element, "data-row-mode") ?? "infinite",
            MinRows = GetIntAttribute(element, "data-min-rows"),
            MaxRows = GetIntAttribute(element, "data-max-rows"),
            AllowAddRows = GetAttributeValue(element, "data-allow-add-rows") != "false",
            AllowDeleteRows = GetAttributeValue(element, "data-allow-delete-rows") != "false",
            RowKey = GetAttributeValue(element, "data-row-key")?.Split(',', StringSplitOptions.RemoveEmptyEntries),
            When = GetAttributeValue(element, "data-when")
        };

        // Parse columns from thead > tr > th[data-col]
        table.Columns = ParseColumns(element);

        // Parse aggregates from tfoot
        table.Aggregates = ParseAggregates(element);

        // Parse print configuration
        table.PrintConfig = ParsePrintConfig(element);

        return table;
    }

    private List<Column> ParseColumns(IElement tableElement)
    {
        var columns = new List<Column>();
        var headers = tableElement.QuerySelectorAll("thead th[data-col], thead td[data-col]");

        foreach (var header in headers)
        {
            var column = new Column
            {
                Name = GetAttributeValue(header, "data-col") ?? throw new InvalidOperationException("Column must have data-col attribute"),
                Label = GetAttributeValue(header, "data-label") ?? header.TextContent.Trim(),
                DataType = GetAttributeValue(header, "data-type") ?? "string",
                Required = GetAttributeValue(header, "data-required") == "true",
                Readonly = GetAttributeValue(header, "data-readonly") == "true",
                Unit = GetAttributeValue(header, "data-unit"),
                Format = GetAttributeValue(header, "data-format"),
                EnumValues = GetAttributeValue(header, "data-enum")?.Split('|', StringSplitOptions.RemoveEmptyEntries),
                Formula = GetAttributeValue(header, "data-formula"),
                DefaultValue = GetAttributeValue(header, "data-default"),
                Min = GetAttributeValue(header, "data-min"),
                Max = GetAttributeValue(header, "data-max"),
                Pattern = GetAttributeValue(header, "data-pattern"),
                Width = GetAttributeValue(header, "data-width"),
                Align = GetAttributeValue(header, "data-align"),
                VAlign = GetAttributeValue(header, "data-valign")
            };

            columns.Add(column);
        }

        return columns;
    }

    private List<Aggregate> ParseAggregates(IElement tableElement)
    {
        var aggregates = new List<Aggregate>();
        var aggElements = tableElement.QuerySelectorAll("tfoot [data-agg]");

        foreach (var aggElement in aggElements)
        {
            var aggAttr = GetAttributeValue(aggElement, "data-agg");
            if (string.IsNullOrEmpty(aggAttr))
            {
                continue;
            }

            // Parse format: "sum(column)" or "avg(column)" or "count()"
            var match = System.Text.RegularExpressions.Regex.Match(aggAttr, @"^(\w+)\((\w*)\)$");
            if (!match.Success)
            {
                continue;
            }

            var aggregate = new Aggregate
            {
                Function = match.Groups[1].Value,
                Column = match.Groups[2].Value,
                TargetId = GetAttributeValue(aggElement, "data-target")?.TrimStart('#') ?? string.Empty
            };

            aggregates.Add(aggregate);
        }

        return aggregates;
    }

    private PrintConfig? ParsePrintConfig(IElement element)
    {
        // Check if any print attributes exist
        if (!element.HasAttribute("data-print-repeat-head-rows") &&
            !element.HasAttribute("data-width") &&
            !element.HasAttribute("data-pagebreak"))
        {
            return null;
        }

        return new PrintConfig
        {
            RepeatHeadRows = GetIntAttribute(element, "data-print-repeat-head-rows") ?? 0,
            PageBreak = GetAttributeValue(element, "data-pagebreak"),
            KeepTogether = GetAttributeValue(element, "data-keep-together") == "true"
        };
    }

    private static string? GetAttributeValue(IElement element, string attributeName)
    {
        return element.GetAttribute(attributeName);
    }

    private static int? GetIntAttribute(IElement element, string attributeName)
    {
        var value = element.GetAttribute(attributeName);
        return int.TryParse(value, out var result) ? result : null;
    }
}
