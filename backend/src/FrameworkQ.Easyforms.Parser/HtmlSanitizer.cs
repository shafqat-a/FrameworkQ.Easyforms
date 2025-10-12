namespace FrameworkQ.Easyforms.Parser;

using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using System.Text;

/// <summary>
/// HTML sanitizer using strict allowlist approach
/// Prevents XSS by removing scripts and dangerous attributes
/// </summary>
public class HtmlSanitizer
{
    private static readonly HashSet<string> AllowedTags = new(StringComparer.OrdinalIgnoreCase)
    {
        "form", "input", "select", "option", "textarea", "button", "label",
        "div", "section", "aside", "article", "header", "footer", "main",
        "table", "thead", "tbody", "tfoot", "tr", "th", "td", "colgroup", "col",
        "ol", "ul", "li",
        "h1", "h2", "h3", "h4", "h5", "h6",
        "p", "span", "strong", "em", "br",
        "canvas"
    };

    private static readonly HashSet<string> AllowedAttributes = new(StringComparer.OrdinalIgnoreCase)
    {
        "id", "name", "type", "value", "class", "style",
        "for", "colspan", "rowspan",
        "required", "readonly", "disabled",
        "min", "max", "step", "pattern", "placeholder",
        "minlength", "maxlength",
        "rows", "cols",
        "multiple", "selected", "checked",
        "width", "height",
        "lang", "dir", "title"
    };

    public string Sanitize(string htmlContent)
    {
        if (string.IsNullOrWhiteSpace(htmlContent))
        {
            throw new ArgumentException("HTML content cannot be empty", nameof(htmlContent));
        }

        // Parse HTML
        var config = Configuration.Default;
        var context = BrowsingContext.New(config);
        var document = context.OpenAsync(req => req.Content(htmlContent)).Result;

        // Remove dangerous elements
        RemoveDangerousElements(document);

        // Sanitize attributes
        SanitizeAttributes(document);

        // Return sanitized HTML
        return document.DocumentElement.OuterHtml;
    }

    private void RemoveDangerousElements(IDocument document)
    {
        // Remove all script tags
        var scripts = document.QuerySelectorAll("script").ToList();
        foreach (var script in scripts)
        {
            script.Remove();
        }

        // Remove other dangerous tags
        var dangerousTags = new[] { "iframe", "object", "embed", "link", "meta", "base", "applet" };
        foreach (var tagName in dangerousTags)
        {
            var elements = document.QuerySelectorAll(tagName).ToList();
            foreach (var element in elements)
            {
                element.Remove();
            }
        }

        // Remove elements not in allowlist
        var allElements = document.QuerySelectorAll("*").ToList();
        foreach (var element in allElements)
        {
            if (!AllowedTags.Contains(element.TagName))
            {
                // Keep children but remove the element itself
                var parent = element.ParentElement;
                if (parent != null)
                {
                    while (element.FirstChild != null)
                    {
                        parent.InsertBefore(element.FirstChild, element);
                    }
                    element.Remove();
                }
            }
        }
    }

    private void SanitizeAttributes(IDocument document)
    {
        var allElements = document.QuerySelectorAll("*").ToList();

        foreach (var element in allElements)
        {
            var attributesToRemove = new List<string>();

            foreach (var attr in element.Attributes)
            {
                var attrName = attr.Name.ToLower();

                // Always allow data-* attributes (used for DSL semantics)
                if (attrName.StartsWith("data-"))
                {
                    // But check for dangerous values
                    if (ContainsDangerousValue(attr.Value))
                    {
                        attributesToRemove.Add(attr.Name);
                    }
                    continue;
                }

                // Check if attribute is in allowlist
                if (!AllowedAttributes.Contains(attrName))
                {
                    attributesToRemove.Add(attr.Name);
                    continue;
                }

                // Check for event handlers
                if (attrName.StartsWith("on"))
                {
                    attributesToRemove.Add(attr.Name);
                    continue;
                }

                // Check for dangerous protocols in href/src
                if ((attrName == "href" || attrName == "src") &&
                    ContainsDangerousProtocol(attr.Value))
                {
                    attributesToRemove.Add(attr.Name);
                }
            }

            // Remove dangerous attributes
            foreach (var attrName in attributesToRemove)
            {
                element.RemoveAttribute(attrName);
            }
        }
    }

    private bool ContainsDangerousValue(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return false;
        }

        var lowerValue = value.ToLower();

        // Check for script injection patterns
        return lowerValue.Contains("javascript:") ||
               lowerValue.Contains("data:text/html") ||
               lowerValue.Contains("<script") ||
               lowerValue.Contains("onerror=") ||
               lowerValue.Contains("onload=");
    }

    private bool ContainsDangerousProtocol(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return false;
        }

        var lowerValue = value.ToLower().Trim();

        return lowerValue.StartsWith("javascript:") ||
               lowerValue.StartsWith("data:") ||
               lowerValue.StartsWith("vbscript:");
    }
}
