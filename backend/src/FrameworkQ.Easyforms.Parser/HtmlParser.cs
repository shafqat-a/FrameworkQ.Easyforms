namespace FrameworkQ.Easyforms.Parser;

using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using FrameworkQ.Easyforms.Core.Interfaces;
using FrameworkQ.Easyforms.Core.Models;
using FrameworkQ.Easyforms.Parser.WidgetParsers;

/// <summary>
/// HTML form parser using AngleSharp
/// </summary>
public class HtmlParser : IFormParser
{
    private readonly IBrowsingContext _context;
    private readonly HtmlSanitizer _sanitizer;

    public HtmlParser()
    {
        var config = Configuration.Default;
        _context = BrowsingContext.New(config);
        _sanitizer = new HtmlSanitizer();
    }

    public async Task<FormDefinition> ParseAsync(string htmlContent)
    {
        // Sanitize HTML first
        var sanitized = _sanitizer.Sanitize(htmlContent);

        // Parse HTML
        var document = await _context.OpenAsync(req => req.Content(sanitized));
        var formElement = document.QuerySelector("form[data-form]") as IHtmlFormElement;

        if (formElement == null)
        {
            throw new InvalidOperationException("No form element with data-form attribute found");
        }

        // Parse form-level attributes
        var formDef = new FormDefinition
        {
            Id = GetAttributeValue(formElement, "data-form") ?? throw new InvalidOperationException("Form must have data-form attribute"),
            Title = GetAttributeValue(formElement, "data-title") ?? string.Empty,
            Version = GetAttributeValue(formElement, "data-version") ?? "1.0",
            Locales = GetAttributeValue(formElement, "data-locale")?.Split(',', StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>(),
            StorageMode = GetAttributeValue(formElement, "data-store") ?? "jsonb",
            Tags = GetAttributeValue(formElement, "data-tags")?.Split(',', StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>(),
            HtmlSource = sanitized,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Parse pages
        formDef.Pages = ParsePages(formElement);

        return formDef;
    }

    public async Task<ValidationResult> ValidateAsync(string htmlContent)
    {
        var result = new ValidationResult { IsValid = true };

        try
        {
            await ParseAsync(htmlContent);
        }
        catch (Exception ex)
        {
            result.IsValid = false;
            result.Errors.Add(ex.Message);
        }

        return result;
    }

    public string Sanitize(string htmlContent)
    {
        return _sanitizer.Sanitize(htmlContent);
    }

    private List<Page> ParsePages(IElement formElement)
    {
        var pages = new List<Page>();
        var pageElements = formElement.QuerySelectorAll("[data-page]");

        if (!pageElements.Any())
        {
            // No explicit pages, treat entire form as single page
            var defaultPage = new Page
            {
                Id = "page-1",
                Title = "Main",
                Order = 0,
                Sections = ParseSections(formElement)
            };
            pages.Add(defaultPage);
            return pages;
        }

        var order = 0;
        foreach (var pageElement in pageElements)
        {
            var page = new Page
            {
                Id = pageElement.GetAttribute("id") ?? $"page-{order + 1}",
                Title = GetAttributeValue(pageElement, "data-title") ?? $"Page {order + 1}",
                Order = order++,
                Sections = ParseSections(pageElement)
            };
            pages.Add(page);
        }

        return pages;
    }

    private List<Section> ParseSections(IElement containerElement)
    {
        var sections = new List<Section>();
        var sectionElements = containerElement.QuerySelectorAll("[data-section]");

        if (!sectionElements.Any())
        {
            // No explicit sections, treat container as single section
            var defaultSection = new Section
            {
                Id = "main",
                Title = "Main",
                Order = 0,
                Widgets = ParseWidgets(containerElement)
            };
            sections.Add(defaultSection);
            return sections;
        }

        var order = 0;
        foreach (var sectionElement in sectionElements)
        {
            var section = new Section
            {
                Id = sectionElement.GetAttribute("id") ?? $"section-{order + 1}",
                Title = GetAttributeValue(sectionElement, "data-title") ?? $"Section {order + 1}",
                NumberingScheme = GetAttributeValue(sectionElement, "data-numbering") ?? "none",
                Level = int.TryParse(GetAttributeValue(sectionElement, "data-level"), out var level) ? level : 0,
                Collapsible = GetAttributeValue(sectionElement, "data-collapsible") == "true",
                Collapsed = GetAttributeValue(sectionElement, "data-collapsed") == "true",
                Order = order++,
                Widgets = ParseWidgets(sectionElement)
            };
            sections.Add(section);
        }

        return sections;
    }

    private List<Widget> ParseWidgets(IElement containerElement)
    {
        var widgets = new List<Widget>();

        // Parse field widgets (input, select, textarea with name attribute)
        var fieldParser = new FieldParser();
        var fieldElements = containerElement.QuerySelectorAll("input[name], select[name], textarea[name]");

        foreach (var fieldElement in fieldElements)
        {
            // Skip if already inside a parsed group or table
            if (IsInsideWidget(fieldElement, containerElement))
            {
                continue;
            }

            var field = fieldParser.Parse(fieldElement);
            if (field != null)
            {
                widgets.Add(field);
            }
        }

        // Parse group widgets
        var groupParser = new GroupParser();
        var groupElements = containerElement.QuerySelectorAll("[data-group]");

        foreach (var groupElement in groupElements)
        {
            var group = groupParser.Parse(groupElement);
            if (group != null)
            {
                widgets.Add(group);
            }
        }

        // Parse table widgets (US2)
        var tableParser = new TableParser();
        var tableElements = containerElement.QuerySelectorAll("table[data-table]");

        foreach (var tableElement in tableElements)
        {
            var table = tableParser.Parse(tableElement);
            if (table != null)
            {
                widgets.Add(table);
            }
        }

        // Parse grid widgets (US2)
        var gridParser = new GridParser();
        var gridElements = containerElement.QuerySelectorAll("table[data-grid]");

        foreach (var gridElement in gridElements)
        {
            var grid = gridParser.Parse(gridElement);
            if (grid != null)
            {
                widgets.Add(grid);
            }
        }

        // Parse composite widgets (custom controls)
        var compositeElements = containerElement.QuerySelectorAll("[data-composite]");
        foreach (var comp in compositeElements)
        {
            if (IsInsideWidget(comp, containerElement))
            {
                continue;
            }

            var composite = ParseComposite(comp);
            if (composite != null)
            {
                widgets.Add(composite);
            }
        }

        // TODO: Parse other widget types in subsequent user stories
        // - Signature (US6)
        // - Checklist, RadioGroup, CheckboxGroup, etc.

        return widgets;
    }

    private bool IsInsideWidget(IElement element, IElement containerElement)
    {
        var parent = element.ParentElement;
        while (parent != null && parent != containerElement)
        {
            if (parent.HasAttribute("data-group") ||
                parent.HasAttribute("data-table") ||
                parent.HasAttribute("data-grid") ||
                parent.HasAttribute("data-composite"))
            {
                return true;
            }
            parent = parent.ParentElement;
        }
        return false;
    }

    private static string? GetAttributeValue(IElement element, string attributeName)
    {
        return element.GetAttribute(attributeName);
    }

    private FrameworkQ.Easyforms.Core.Models.CompositeWidget? ParseComposite(IElement element)
    {
        var name = GetAttributeValue(element, "data-composite");
        if (string.IsNullOrWhiteSpace(name)) return null;

        var composite = new FrameworkQ.Easyforms.Core.Models.CompositeWidget
        {
            Id = element.GetAttribute("id") ?? $"comp-{Guid.NewGuid().ToString("N")[..8]}",
            Name = name,
            IsContainer = (GetAttributeValue(element, "data-container") == "true"),
            When = GetAttributeValue(element, "data-when")
        };

        // Collect data-prop-* attributes
        foreach (var attr in element.Attributes)
        {
            var aname = attr.Name.ToLower();
            if (aname.StartsWith("data-prop-"))
            {
                var propName = aname.Substring("data-prop-".Length);
                composite.Properties[propName] = attr.Value;
            }
        }

        // If container, we could parse child widgets within, but often the runtime expands the template.
        // To avoid double parsing, only parse direct children if explicit flag set
        if (composite.IsContainer && element.HasAttribute("data-parse-children"))
        {
            composite.Children = ParseWidgets(element);
        }

        return composite;
    }
}
