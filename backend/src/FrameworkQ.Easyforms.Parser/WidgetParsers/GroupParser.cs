namespace FrameworkQ.Easyforms.Parser.WidgetParsers;

using AngleSharp.Dom;
using FrameworkQ.Easyforms.Core.Models;

/// <summary>
/// Parser for Group widgets (containers with data-group attribute)
/// </summary>
public class GroupParser
{
    public Group? Parse(IElement element)
    {
        if (!element.HasAttribute("data-group"))
        {
            return null;
        }

        var group = new Group
        {
            Id = element.GetAttribute("id") ?? $"group-{Guid.NewGuid().ToString("N")[..8]}",
            Layout = GetAttributeValue(element, "data-layout") ?? "columns:1",
            When = GetAttributeValue(element, "data-when")
        };

        // Parse child fields
        var fieldParser = new FieldParser();
        var fieldElements = element.QuerySelectorAll("input[name], select[name], textarea[name]");

        foreach (var fieldElement in fieldElements)
        {
            var field = fieldParser.Parse(fieldElement);
            if (field != null)
            {
                group.Fields.Add(field);
            }
        }

        return group;
    }

    private static string? GetAttributeValue(IElement element, string attributeName)
    {
        return element.GetAttribute(attributeName);
    }
}
