namespace FrameworkQ.Easyforms.Runtime;

using FrameworkQ.Easyforms.Core.Models;
using System.Text.Json;

/// <summary>
/// Processes form submissions (validation, storage, reporting data extraction)
/// </summary>
public class SubmissionProcessor
{
    private readonly ValidationEngine _validationEngine;

    public SubmissionProcessor()
    {
        _validationEngine = new ValidationEngine();
    }

    /// <summary>
    /// Validate submission data
    /// </summary>
    public ValidationResult ValidateSubmission(FormDefinition formDefinition, Dictionary<string, object?> submissionData)
    {
        return _validationEngine.Validate(formDefinition, submissionData);
    }

    /// <summary>
    /// Process and save submission
    /// </summary>
    public async Task<SubmissionResult> ProcessSubmission(FormSubmissionRequest request)
    {
        var result = new SubmissionResult
        {
            InstanceId = Guid.NewGuid(),
            SubmittedAt = DateTime.UtcNow,
            Success = true
        };

        try
        {
            // TODO: Save to database (in-memory for now)
            // 1. Insert into form_instances table
            // 2. Extract and insert reporting table data

            _inMemorySubmissions[result.InstanceId] = new SubmissionRecord
            {
                InstanceId = result.InstanceId,
                FormId = request.FormId,
                FormVersion = request.FormVersion,
                SubmittedAt = result.SubmittedAt,
                SubmittedBy = request.SubmittedBy,
                Status = request.Status,
                HeaderContext = request.HeaderContext,
                RawData = request.Data
            };
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.Errors.Add(ex.Message);
        }

        return result;
    }

    /// <summary>
    /// Get submission by ID
    /// </summary>
    public SubmissionRecord? GetSubmission(Guid instanceId)
    {
        return _inMemorySubmissions.TryGetValue(instanceId, out var submission) ? submission : null;
    }

    /// <summary>
    /// Update submission (drafts only)
    /// </summary>
    public async Task<SubmissionResult> UpdateSubmission(Guid instanceId, Dictionary<string, object?> data)
    {
        var result = new SubmissionResult { Success = false };

        if (!_inMemorySubmissions.TryGetValue(instanceId, out var submission))
        {
            result.Errors.Add("Submission not found");
            return result;
        }

        if (submission.Status != "draft")
        {
            result.Errors.Add("Can only update draft submissions");
            return result;
        }

        submission.RawData = data;
        submission.UpdatedAt = DateTime.UtcNow;

        result.Success = true;
        result.InstanceId = instanceId;
        result.SubmittedAt = submission.UpdatedAt ?? DateTime.UtcNow;

        return result;
    }

    /// <summary>
    /// Delete submission (drafts only)
    /// </summary>
    public bool DeleteSubmission(Guid instanceId)
    {
        if (!_inMemorySubmissions.TryGetValue(instanceId, out var submission))
        {
            return false;
        }

        if (submission.Status != "draft")
        {
            return false;
        }

        return _inMemorySubmissions.Remove(instanceId);
    }

    /// <summary>
    /// Query submissions
    /// </summary>
    public List<SubmissionRecord> QuerySubmissions(string? formId = null, string? status = null, string? submittedBy = null)
    {
        var query = _inMemorySubmissions.Values.AsEnumerable();

        if (!string.IsNullOrEmpty(formId))
        {
            query = query.Where(s => s.FormId == formId);
        }

        if (!string.IsNullOrEmpty(status))
        {
            query = query.Where(s => s.Status == status);
        }

        if (!string.IsNullOrEmpty(submittedBy))
        {
            query = query.Where(s => s.SubmittedBy == submittedBy);
        }

        return query.OrderByDescending(s => s.SubmittedAt).ToList();
    }

    // In-memory storage (TODO: Replace with database)
    private static readonly Dictionary<Guid, SubmissionRecord> _inMemorySubmissions = new();
}

public class FormSubmissionRequest
{
    public string FormId { get; set; } = string.Empty;
    public string FormVersion { get; set; } = string.Empty;
    public string Status { get; set; } = "submitted";
    public string? SubmittedBy { get; set; }
    public Dictionary<string, object?>? HeaderContext { get; set; }
    public Dictionary<string, object?> Data { get; set; } = new();
}

public class SubmissionResult
{
    public bool Success { get; set; }
    public Guid InstanceId { get; set; }
    public DateTime SubmittedAt { get; set; }
    public List<string> Errors { get; set; } = new();
}

public class SubmissionRecord
{
    public Guid InstanceId { get; set; }
    public string FormId { get; set; } = string.Empty;
    public string FormVersion { get; set; } = string.Empty;
    public DateTime SubmittedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? SubmittedBy { get; set; }
    public string Status { get; set; } = "submitted";
    public Dictionary<string, object?>? HeaderContext { get; set; }
    public Dictionary<string, object?> RawData { get; set; } = new();
}
