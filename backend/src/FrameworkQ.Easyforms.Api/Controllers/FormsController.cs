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
    private readonly FrameworkQ.Easyforms.Api.Storage.IFormStore _formStore;

    public FormsController(
        ILogger<FormsController> logger,
        IFormParser formParser,
        ISchemaExtractor schemaExtractor,
        FrameworkQ.Easyforms.Api.Storage.IFormStore formStore)
    {
        _logger = logger;
        _formParser = formParser;
        _schemaExtractor = schemaExtractor;
        _formStore = formStore;
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

            // Persist form (file-backed store)
            await _formStore.SaveAsync(
                formDef.Id,
                formDef.HtmlSource,
                formDef.SchemaJson,
                new FrameworkQ.Easyforms.Api.Storage.FormMeta
                {
                    Id = formDef.Id,
                    Title = formDef.Title,
                    Version = formDef.Version,
                    Tags = formDef.Tags,
                    CreatedAt = formDef.CreatedAt,
                    UpdatedAt = formDef.UpdatedAt
                }
            );

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
    public async Task<IActionResult> GetForm(string formId, [FromQuery] string? version = null)
    {
        _logger.LogInformation("Retrieving form: {FormId}", formId);

        var meta = await _formStore.GetMetaAsync(formId);
        var schema = await _formStore.GetSchemaAsync(formId);
        if (meta == null || schema == null)
        {
            return NotFound(new { error = new { code = "NOT_FOUND", message = $"Form '{formId}' not found" } });
        }
        return Ok(new
        {
            id = meta.Id,
            title = meta.Title,
            version = meta.Version,
            locales = Array.Empty<string>(),
            storageMode = "jsonb",
            tags = meta.Tags,
            createdAt = meta.CreatedAt,
            updatedAt = meta.UpdatedAt,
            schema = System.Text.Json.JsonDocument.Parse(schema)
        });
    }

    /// <summary>
    /// Get extracted JSON schema
    /// GET /v1/forms/{formId}/schema
    /// </summary>
    [HttpGet("{formId}/schema")]
    public async Task<IActionResult> GetSchema(string formId, [FromQuery] string? version = null)
    {
        _logger.LogInformation("Retrieving schema for form: {FormId}", formId);

        var schema = await _formStore.GetSchemaAsync(formId);
        if (schema == null)
        {
            return NotFound(new { error = new { code = "NOT_FOUND", message = $"Form '{formId}' not found" } });
        }
        return Content(schema, "application/json");
    }

    /// <summary>
    /// Get original HTML template
    /// GET /v1/forms/{formId}/html
    /// </summary>
    [HttpGet("{formId}/html")]
    public async Task<IActionResult> GetHtml(string formId, [FromQuery] string? version = null)
    {
        _logger.LogInformation("Retrieving HTML for form: {FormId}", formId);

        var html = await _formStore.GetHtmlAsync(formId);
        if (html == null)
        {
            return NotFound(new { error = new { code = "NOT_FOUND", message = $"Form '{formId}' not found" } });
        }
        return Content(html, "text/html");
    }

    /// <summary>
    /// List all forms (bonus endpoint)
    /// GET /v1/forms
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> ListForms([FromQuery] string? tags = null)
    {
        var all = await _formStore.ListAsync();
        if (!string.IsNullOrEmpty(tags))
        {
            var tagList = tags.Split(',', StringSplitOptions.RemoveEmptyEntries);
            all = all.Where(f => f.Tags.Intersect(tagList).Any()).ToList();
        }
        return Ok(new
        {
            data = all.Select(f => new { id = f.Id, title = f.Title, version = f.Version, tags = f.Tags, updatedAt = f.UpdatedAt }),
            pagination = new { page = 1, limit = 100, total = all.Count }
        });
    }
}
