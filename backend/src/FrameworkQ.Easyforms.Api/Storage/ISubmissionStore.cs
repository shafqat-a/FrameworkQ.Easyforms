namespace FrameworkQ.Easyforms.Api.Storage;

public class SubmissionRecordDto
{
    public Guid InstanceId { get; set; }
    public string FormId { get; set; } = string.Empty;
    public string FormVersion { get; set; } = string.Empty;
    public DateTime SubmittedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? SubmittedBy { get; set; }
    public string Status { get; set; } = "submitted";
    public Dictionary<string, object?>? HeaderContext { get; set; }
    public Dictionary<string, object?> Data { get; set; } = new();
    public Dictionary<string, object?>? CompositeState { get; set; }
}

public interface ISubmissionStore
{
    Task SaveAsync(SubmissionRecordDto submission, CancellationToken ct = default);
    Task<SubmissionRecordDto?> GetAsync(Guid instanceId, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid instanceId, CancellationToken ct = default);
    Task<List<SubmissionRecordDto>> QueryAsync(string? formId = null, string? status = null, string? submittedBy = null, CancellationToken ct = default);
}
