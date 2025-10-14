namespace FrameworkQ.Easyforms.Api.Controllers;

using Microsoft.AspNetCore.Mvc;
using FrameworkQ.Easyforms.Runtime;

/// <summary>
/// API controller for querying submission data and reporting tables
/// </summary>
[ApiController]
[Route("v1/query")]
public class QueryController : ControllerBase
{
    private readonly ILogger<QueryController> _logger;
    private readonly FrameworkQ.Easyforms.Api.Storage.ISubmissionStore _submissionStore;

    public QueryController(ILogger<QueryController> logger, FrameworkQ.Easyforms.Api.Storage.ISubmissionStore submissionStore)
    {
        _logger = logger;
        _submissionStore = submissionStore;
    }

    /// <summary>
    /// Query form submissions with filtering
    /// GET /v1/query/submissions
    /// </summary>
    [HttpGet("submissions")]
    public async Task<IActionResult> QuerySubmissions(
        [FromQuery] string? formId,
        [FromQuery] string? status,
        [FromQuery] string? submittedBy,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        [FromQuery] int page = 1,
        [FromQuery] int limit = 20)
    {
        _logger.LogInformation("Querying submissions: formId={FormId}, status={Status}", formId, status);

        try
        {
            var submissions = await _submissionStore.QueryAsync(formId, status, submittedBy);

            // Date filtering
            if (fromDate.HasValue)
            {
                submissions = submissions.Where(s => s.SubmittedAt >= fromDate.Value).ToList();
            }

            if (toDate.HasValue)
            {
                submissions = submissions.Where(s => s.SubmittedAt <= toDate.Value).ToList();
            }

            // Pagination
            var total = submissions.Count;
            var paged = submissions.Skip((page - 1) * limit).Take(limit).ToList();

            return Ok(new
            {
                data = paged.Select(s => new
                {
                    instanceId = s.InstanceId,
                    formId = s.FormId,
                    formVersion = s.FormVersion,
                    submittedAt = s.SubmittedAt,
                    submittedBy = s.SubmittedBy,
                    status = s.Status
                }),
                pagination = new
                {
                    page = page,
                    limit = limit,
                    total = total,
                    totalPages = (int)Math.Ceiling(total / (double)limit)
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to query submissions");
            return StatusCode(500, new
            {
                error = new
                {
                    code = "QUERY_FAILED",
                    message = ex.Message,
                    correlationId = HttpContext.TraceIdentifier
                }
            });
        }
    }

    /// <summary>
    /// Query reporting table data
    /// GET /v1/query/reporting/{tableName}
    /// </summary>
    [HttpGet("reporting/{tableName}")]
    public IActionResult QueryReportingTable(
        string tableName,
        [FromQuery] Guid? instanceId,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        [FromQuery] int page = 1,
        [FromQuery] int limit = 100)
    {
        _logger.LogInformation("Querying reporting table: {TableName}", tableName);

        // TODO: Implement actual database querying
        // For now, return empty result
        return Ok(new
        {
            data = new object[] { },
            pagination = new
            {
                page = page,
                limit = limit,
                total = 0,
                totalPages = 0
            }
        });
    }
}
