namespace FrameworkQ.Easyforms.Parser.WidgetParsers;

using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using FrameworkQ.Easyforms.Core.Models;

/// <summary>
/// Parser for Field widgets (input, select, textarea)
/// </summary>
public class FieldParser
{
    public Field? Parse(IElement element)
    {
        var name = element.GetAttribute("name");
        if (string.IsNullOrEmpty(name))
        {
            return null;
        }

        var field = new Field
        {
            Id = element.GetAttribute("id") ?? name,
            Name = name,
            DataType = GetAttributeValue(element, "data-type") ?? InferDataType(element),
            Label = GetLabel(element),
            Required = HasAttribute(element, "required") || GetAttributeValue(element, "data-required") == "true",
            Readonly = HasAttribute(element, "readonly") || GetAttributeValue(element, "data-readonly") == "true",
            DefaultValue = GetAttributeValue(element, "data-default") ?? GetAttributeValue(element, "value"),
            Unit = GetAttributeValue(element, "data-unit"),
            Pattern = GetAttributeValue(element, "pattern") ?? GetAttributeValue(element, "data-pattern"),
            Min = GetAttributeValue(element, "min") ?? GetAttributeValue(element, "data-min"),
            Max = GetAttributeValue(element, "max") ?? GetAttributeValue(element, "data-max"),
            Format = GetAttributeValue(element, "data-format"),
            EnumValues = GetEnumValues(element),
            Compute = GetAttributeValue(element, "data-compute"),
            Override = GetAttributeValue(element, "data-override") == "true",
            When = GetAttributeValue(element, "data-when")
        };

        // Parse validation rules
        field.ValidationRules = ParseValidationRules(element, field);

        // Parse fetch configuration (if present)
        if (HasAttribute(element, "data-fetch"))
        {
            field.FetchConfig = ParseFetchConfig(element);
        }

        return field;
    }

    private string InferDataType(IElement element)
    {
        var tagName = element.TagName.ToLower();
        var inputType = GetAttributeValue(element, "type")?.ToLower();

        return (tagName, inputType) switch
        {
            ("textarea", _) => "text",
            ("select", _) => "enum",
            ("input", "number") => "integer",
            ("input", "email") => "string",
            ("input", "tel") => "string",
            ("input", "date") => "date",
            ("input", "time") => "time",
            ("input", "datetime-local") => "datetime",
            ("input", "checkbox") => "bool",
            _ => "string"
        };
    }

    private string? GetLabel(IElement element)
    {
        // Check data-label attribute first
        var dataLabel = GetAttributeValue(element, "data-label");
        if (!string.IsNullOrEmpty(dataLabel))
        {
            return dataLabel;
        }

        // Find associated label element
        var id = element.GetAttribute("id");
        if (!string.IsNullOrEmpty(id))
        {
            var label = element.Owner?.QuerySelector($"label[for='{id}']");
            if (label != null)
            {
                return label.TextContent.Trim();
            }
        }

        // Check if element is inside a label
        var parentLabel = element.ParentElement;
        while (parentLabel != null && parentLabel.TagName.ToLower() != "label")
        {
            parentLabel = parentLabel.ParentElement;
        }

        if (parentLabel != null)
        {
            return parentLabel.TextContent.Trim();
        }

        return null;
    }

    private string[]? GetEnumValues(IElement element)
    {
        var dataEnum = GetAttributeValue(element, "data-enum");
        if (!string.IsNullOrEmpty(dataEnum))
        {
            return dataEnum.Split('|', StringSplitOptions.RemoveEmptyEntries);
        }

        // For select elements, extract options
        if (element is IHtmlSelectElement selectElement)
        {
            return selectElement.Options
                .Select(opt => opt.Value)
                .Where(val => !string.IsNullOrEmpty(val))
                .ToArray();
        }

        return null;
    }

    private List<ValidationRule> ParseValidationRules(IElement element, Field field)
    {
        var rules = new List<ValidationRule>();

        // Required
        if (field.Required)
        {
            rules.Add(new ValidationRule
            {
                Type = "required",
                Message = GetAttributeValue(element, "data-error-required") ?? "This field is required",
                Severity = "error",
                ValidateOn = GetAttributeValue(element, "data-validate-on") ?? "change"
            });
        }

        // Pattern
        if (!string.IsNullOrEmpty(field.Pattern))
        {
            rules.Add(new ValidationRule
            {
                Type = "pattern",
                Expression = field.Pattern,
                Message = GetAttributeValue(element, "data-error-pattern") ?? "Value does not match required pattern",
                Severity = "error"
            });
        }

        // Min/Max
        if (!string.IsNullOrEmpty(field.Min) || !string.IsNullOrEmpty(field.Max))
        {
            if (!string.IsNullOrEmpty(field.Min))
            {
                rules.Add(new ValidationRule
                {
                    Type = "min",
                    Expression = field.Min,
                    Message = GetAttributeValue(element, "data-error-min") ?? $"Value must be at least {field.Min}"
                });
            }

            if (!string.IsNullOrEmpty(field.Max))
            {
                rules.Add(new ValidationRule
                {
                    Type = "max",
                    Expression = field.Max,
                    Message = GetAttributeValue(element, "data-error-max") ?? $"Value must be at most {field.Max}"
                });
            }
        }

        // Conditional required
        var requiredWhen = GetAttributeValue(element, "data-required-when");
        if (!string.IsNullOrEmpty(requiredWhen))
        {
            rules.Add(new ValidationRule
            {
                Type = "required-when",
                Expression = requiredWhen,
                Message = GetAttributeValue(element, "data-error-required") ?? "This field is required"
            });
        }

        return rules;
    }

    private FetchConfig? ParseFetchConfig(IElement element)
    {
        var dataFetch = GetAttributeValue(element, "data-fetch");
        if (string.IsNullOrEmpty(dataFetch))
        {
            return null;
        }

        // Parse method:url format (e.g., "GET:/api/endpoint")
        var parts = dataFetch.Split(':', 2);
        var method = parts.Length > 1 ? parts[0] : "GET";
        var url = parts.Length > 1 ? parts[1] : parts[0];

        return new FetchConfig
        {
            Method = method,
            Url = url,
            FetchOn = GetAttributeValue(element, "data-fetch-on") ?? "focus",
            MinChars = int.TryParse(GetAttributeValue(element, "data-min-chars"), out var minChars) ? minChars : 0,
            DebounceMs = int.TryParse(GetAttributeValue(element, "data-fetch-debounce")?.TrimEnd("ms".ToCharArray()), out var debounce) ? debounce : 300,
            Map = GetAttributeValue(element, "data-fetch-map"),
            Cache = GetAttributeValue(element, "data-fetch-cache") ?? "session",
            Depends = GetAttributeValue(element, "data-depends")?.Split(',', StringSplitOptions.RemoveEmptyEntries)
        };
    }

    private static string? GetAttributeValue(IElement element, string attributeName)
    {
        return element.GetAttribute(attributeName);
    }

    private static bool HasAttribute(IElement element, string attributeName)
    {
        return element.HasAttribute(attributeName);
    }
}
