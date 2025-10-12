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
    private readonly SubmissionProcessor _processor;

    public SubmissionsController(ILogger<SubmissionsController> logger)
    {
        _logger = logger;
        _processor = new SubmissionProcessor();
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
            // Process submission
            var result = await _processor.ProcessSubmission(request);

            if (!result.Success)
            {
                return BadRequest(new
                {
                    error = new
                    {
                        code = "SUBMISSION_FAILED",
                        message = "Submission failed",
                        details = result.Errors
                    }
                });
            }

            return CreatedAtAction(
                nameof(GetSubmission),
                new { instanceId = result.InstanceId },
                new
                {
                    instanceId = result.InstanceId,
                    submittedAt = result.SubmittedAt
                }
            );
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

    /// <summary>
    /// Get submission by ID
    /// GET /v1/submissions/{instanceId}
    /// </summary>
    [HttpGet("{instanceId}")]
    public IActionResult GetSubmission(Guid instanceId)
    {
        _logger.LogInformation("Retrieving submission: {InstanceId}", instanceId);

        var submission = _processor.GetSubmission(instanceId);

        if (submission == null)
        {
            return NotFound(new
            {
                error = new
                {
                    code = "NOT_FOUND",
                    message = $"Submission '{instanceId}' not found"
                }
            });
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
            data = submission.RawData
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

        var result = await _processor.UpdateSubmission(instanceId, request.Data);

        if (!result.Success)
        {
            return BadRequest(new
            {
                error = new
                {
                    code = "UPDATE_FAILED",
                    message = result.Errors.FirstOrDefault() ?? "Update failed"
                }
            });
        }

        return Ok(new
        {
            instanceId = result.InstanceId,
            updatedAt = result.SubmittedAt
        });
    }

    /// <summary>
    /// Delete submission (drafts only)
    /// DELETE /v1/submissions/{instanceId}
    /// </summary>
    [HttpDelete("{instanceId}")]
    public IActionResult DeleteSubmission(Guid instanceId)
    {
        _logger.LogInformation("Deleting submission: {InstanceId}", instanceId);

        var deleted = _processor.DeleteSubmission(instanceId);

        if (!deleted)
        {
            return BadRequest(new
            {
                error = new
                {
                    code = "DELETE_FAILED",
                    message = "Cannot delete submission (not found or not a draft)"
                }
            });
        }

        return NoContent();
    }
}

public class UpdateSubmissionRequest
{
    public Dictionary<string, object?> Data { get; set; } = new();
    public string? Status { get; set; }
}
