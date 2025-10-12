namespace FrameworkQ.Easyforms.Api.Controllers;

using Microsoft.AspNetCore.Mvc;
using FrameworkQ.Easyforms.Core.Interfaces;
using FrameworkQ.Easyforms.Core.Models;
using FrameworkQ.Easyforms.Parser;

/// <summary>
/// API controller for form template management
/// </summary>
[ApiController]
[Route("v1/forms")]
public class FormsController : ControllerBase
{
    private readonly ILogger<FormsController> _logger;
    private readonly IFormParser _formParser;
    private readonly ISchemaExtractor _schemaExtractor;

    // In-memory storage for demo (TODO: Replace with database in US4)
    private static readonly Dictionary<string, FormDefinition> _forms = new();

    public FormsController(ILogger<FormsController> logger)
    {
        _logger = logger;
        _formParser = new HtmlParser();
        _schemaExtractor = new SchemaBuilder();
    }

    /// <summary>
    /// Upload and parse a new form template
    /// POST /v1/forms
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> UploadForm(IFormFile htmlFile)
    {
        _logger.LogInformation("Uploading new form template: {FileName}", htmlFile.FileName);

        if (htmlFile == null || htmlFile.Length == 0)
        {
            return BadRequest(new { error = new { code = "INVALID_FILE", message = "No file uploaded" } });
        }

        if (!htmlFile.FileName.EndsWith(".html", StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest(new { error = new { code = "INVALID_FILE_TYPE", message = "File must be HTML" } });
        }

        try
        {
            // Read HTML content
            using var reader = new StreamReader(htmlFile.OpenReadStream());
            var htmlContent = await reader.ReadToEndAsync();

            // Parse form
            var formDef = await _formParser.ParseAsync(htmlContent);

            // Extract schema
            formDef.SchemaJson = await _schemaExtractor.ExtractSchemaAsync(formDef);

            // Store form (in-memory for now)
            _forms[formDef.Id] = formDef;

            _logger.LogInformation("Form uploaded successfully: {FormId} v{Version}", formDef.Id, formDef.Version);

            return CreatedAtAction(
                nameof(GetForm),
                new { formId = formDef.Id },
                new
                {
                    id = formDef.Id,
                    title = formDef.Title,
                    version = formDef.Version,
                    tags = formDef.Tags,
                    createdAt = formDef.CreatedAt
                }
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to parse form template");
            return BadRequest(new
            {
                error = new
                {
                    code = "PARSE_ERROR",
                    message = ex.Message,
                    correlationId = HttpContext.TraceIdentifier
                }
            });
        }
    }

    /// <summary>
    /// Get form details by ID
    /// GET /v1/forms/{formId}
    /// </summary>
    [HttpGet("{formId}")]
    public IActionResult GetForm(string formId, [FromQuery] string? version = null)
    {
        _logger.LogInformation("Retrieving form: {FormId}", formId);

        if (!_forms.TryGetValue(formId, out var formDef))
        {
            return NotFound(new
            {
                error = new
                {
                    code = "NOT_FOUND",
                    message = $"Form '{formId}' not found"
                }
            });
        }

        return Ok(new
        {
            id = formDef.Id,
            title = formDef.Title,
            version = formDef.Version,
            locales = formDef.Locales,
            storageMode = formDef.StorageMode,
            tags = formDef.Tags,
            createdAt = formDef.CreatedAt,
            updatedAt = formDef.UpdatedAt,
            schema = System.Text.Json.JsonDocument.Parse(formDef.SchemaJson)
        });
    }

    /// <summary>
    /// Get extracted JSON schema
    /// GET /v1/forms/{formId}/schema
    /// </summary>
    [HttpGet("{formId}/schema")]
    public IActionResult GetSchema(string formId, [FromQuery] string? version = null)
    {
        _logger.LogInformation("Retrieving schema for form: {FormId}", formId);

        if (!_forms.TryGetValue(formId, out var formDef))
        {
            return NotFound(new
            {
                error = new
                {
                    code = "NOT_FOUND",
                    message = $"Form '{formId}' not found"
                }
            });
        }

        return Content(formDef.SchemaJson, "application/json");
    }

    /// <summary>
    /// Get original HTML template
    /// GET /v1/forms/{formId}/html
    /// </summary>
    [HttpGet("{formId}/html")]
    public IActionResult GetHtml(string formId, [FromQuery] string? version = null)
    {
        _logger.LogInformation("Retrieving HTML for form: {FormId}", formId);

        if (!_forms.TryGetValue(formId, out var formDef))
        {
            return NotFound(new
            {
                error = new
                {
                    code = "NOT_FOUND",
                    message = $"Form '{formId}' not found"
                }
            });
        }

        return Content(formDef.HtmlSource, "text/html");
    }

    /// <summary>
    /// List all forms (bonus endpoint)
    /// GET /v1/forms
    /// </summary>
    [HttpGet]
    public IActionResult ListForms([FromQuery] string? tags = null)
    {
        var forms = _forms.Values.AsEnumerable();

        if (!string.IsNullOrEmpty(tags))
        {
            var tagList = tags.Split(',', StringSplitOptions.RemoveEmptyEntries);
            forms = forms.Where(f => f.Tags.Intersect(tagList).Any());
        }

        return Ok(new
        {
            data = forms.Select(f => new
            {
                id = f.Id,
                title = f.Title,
                version = f.Version,
                tags = f.Tags,
                updatedAt = f.UpdatedAt
            }),
            pagination = new
            {
                page = 1,
                limit = 100,
                total = forms.Count()
            }
        });
    }
}
