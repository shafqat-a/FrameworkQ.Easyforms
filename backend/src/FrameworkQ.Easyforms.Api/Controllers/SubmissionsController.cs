namespace FrameworkQ.Easyforms.Api.Controllers;

using Microsoft.AspNetCore.Mvc;
using FrameworkQ.Easyforms.Runtime;
using System.Text.Json;

/// <summary>
/// API controller for form submissions
/// </summary>
[ApiController]
[Route("v1/submissions")]
public class SubmissionsController : ControllerBase
{
    private readonly ILogger<SubmissionsController> _logger;
    private readonly FrameworkQ.Easyforms.Api.Storage.ISubmissionStore _submissionStore;
    private readonly FrameworkQ.Easyforms.Api.Storage.IFormStore _formStore;
    private readonly FrameworkQ.Easyforms.Core.Interfaces.IFormParser _formParser;
    private readonly FrameworkQ.Easyforms.Runtime.ValidationEngine _validationEngine;

    public SubmissionsController(
        ILogger<SubmissionsController> logger,
        FrameworkQ.Easyforms.Api.Storage.ISubmissionStore submissionStore,
        FrameworkQ.Easyforms.Api.Storage.IFormStore formStore,
        FrameworkQ.Easyforms.Core.Interfaces.IFormParser formParser)
    {
        _logger = logger;
        _submissionStore = submissionStore;
        _formStore = formStore;
        _formParser = formParser;
        _validationEngine = new FrameworkQ.Easyforms.Runtime.ValidationEngine();
    }

    /// <summary>
    /// Submit form data
    /// POST /v1/submissions
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Submit([FromBody] FormSubmissionRequest request)
    {
        _logger.LogInformation("Submitting form: {FormId} with status: {Status}", request.FormId, request.Status);

        try
        {
            // Validate against form definition
            var html = await _formStore.GetHtmlAsync(request.FormId);
            if (html == null)
            {
                return NotFound(new { error = new { code = "FORM_NOT_FOUND", message = $"Form '{request.FormId}' not found" } });
            }
            var formDef = await _formParser.ParseAsync(html);
            var validation = _validationEngine.Validate(formDef, request.Data);
            if (!validation.IsValid)
            {
                return BadRequest(new { error = new { code = "VALIDATION_FAILED", message = "Validation failed", details = validation.FieldErrors } });
            }

            var instanceId = Guid.NewGuid();
            var submission = new FrameworkQ.Easyforms.Api.Storage.SubmissionRecordDto
            {
                InstanceId = instanceId,
                FormId = request.FormId,
                FormVersion = request.FormVersion,
                SubmittedAt = DateTime.UtcNow,
                SubmittedBy = request.SubmittedBy,
                Status = string.IsNullOrEmpty(request.Status) ? "submitted" : request.Status,
                HeaderContext = request.HeaderContext,
                Data = request.Data,
                CompositeState = ExtractCompositeState(request.Data)
            };

            await _submissionStore.SaveAsync(submission);

            return CreatedAtAction(nameof(GetSubmission), new { instanceId }, new { instanceId, submittedAt = submission.SubmittedAt });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process submission");
            return StatusCode(500, new
            {
                error = new
                {
                    code = "INTERNAL_ERROR",
                    message = ex.Message,
                    correlationId = HttpContext.TraceIdentifier
                }
            });
        }
    }

    private Dictionary<string, object?>? ExtractCompositeState(Dictionary<string, object?> data)
    {
        if (data != null && data.TryGetValue("_composites", out var comp) && comp is JsonElement je && je.ValueKind != JsonValueKind.Undefined && je.ValueKind != JsonValueKind.Null)
        {
            try
            {
                var dict = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object?>>(je.GetRawText());
                return dict;
            }
            catch { return null; }
        }
        else if (data != null && data.TryGetValue("_composites", out var compObj) && compObj is Dictionary<string, object?> d)
        {
            return d;
        }
        return null;
    }

    /// <summary>
    /// Update composite state for a submission
    /// PUT /v1/submissions/{instanceId}/composites
    /// </summary>
    [HttpPut("{instanceId}/composites")]
    public async Task<IActionResult> UpdateCompositeState(Guid instanceId, [FromBody] Dictionary<string, object?> state)
    {
        var existing = await _submissionStore.GetAsync(instanceId);
        if (existing == null)
        {
            return NotFound(new { error = new { code = "NOT_FOUND", message = "Submission not found" } });
        }
        existing.CompositeState = state;
        existing.UpdatedAt = DateTime.UtcNow;
        await _submissionStore.SaveAsync(existing);
        return Ok(new { instanceId = existing.InstanceId, updatedAt = existing.UpdatedAt });
    }

    /// <summary>
    /// Get submission by ID
    /// GET /v1/submissions/{instanceId}
    /// </summary>
    [HttpGet("{instanceId}")]
    public async Task<IActionResult> GetSubmission(Guid instanceId)
    {
        _logger.LogInformation("Retrieving submission: {InstanceId}", instanceId);

        var submission = await _submissionStore.GetAsync(instanceId);
        if (submission == null)
        {
            return NotFound(new { error = new { code = "NOT_FOUND", message = $"Submission '{instanceId}' not found" } });
        }

        return Ok(new
        {
            instanceId = submission.InstanceId,
            formId = submission.FormId,
            formVersion = submission.FormVersion,
            submittedAt = submission.SubmittedAt,
            submittedBy = submission.SubmittedBy,
            status = submission.Status,
            headerContext = submission.HeaderContext,
            data = submission.Data,
            compositeState = submission.CompositeState
        });
    }

    /// <summary>
    /// Update submission (drafts only)
    /// PUT /v1/submissions/{instanceId}
    /// </summary>
    [HttpPut("{instanceId}")]
    public async Task<IActionResult> UpdateSubmission(Guid instanceId, [FromBody] UpdateSubmissionRequest request)
    {
        _logger.LogInformation("Updating submission: {InstanceId}", instanceId);
        var existing = await _submissionStore.GetAsync(instanceId);
        if (existing == null)
        {
            return NotFound(new { error = new { code = "NOT_FOUND", message = "Submission not found" } });
        }
        if (!string.Equals(existing.Status, "draft", StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest(new { error = new { code = "UPDATE_FAILED", message = "Can only update draft submissions" } });
        }
        existing.Data = request.Data;
        existing.UpdatedAt = DateTime.UtcNow;
        if (!string.IsNullOrEmpty(request.Status)) existing.Status = request.Status;
        await _submissionStore.SaveAsync(existing);
        return Ok(new { instanceId = existing.InstanceId, updatedAt = existing.UpdatedAt });
    }

    /// <summary>
    /// Delete submission (drafts only)
    /// DELETE /v1/submissions/{instanceId}
    /// </summary>
    [HttpDelete("{instanceId}")]
    public async Task<IActionResult> DeleteSubmission(Guid instanceId)
    {
        _logger.LogInformation("Deleting submission: {InstanceId}", instanceId);
        var existing = await _submissionStore.GetAsync(instanceId);
        if (existing == null || !string.Equals(existing.Status, "draft", StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest(new { error = new { code = "DELETE_FAILED", message = "Cannot delete submission (not found or not a draft)" } });
        }
        var deleted = await _submissionStore.DeleteAsync(instanceId);
        return deleted ? NoContent() : BadRequest(new { error = new { code = "DELETE_FAILED", message = "Delete failed" } });
    }
}

public class UpdateSubmissionRequest
{
    public Dictionary<string, object?> Data { get; set; } = new();
    public string? Status { get; set; }
}
