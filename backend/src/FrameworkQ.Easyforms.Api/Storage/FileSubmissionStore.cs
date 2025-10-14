using System.Text.Json;

namespace FrameworkQ.Easyforms.Api.Storage;

public class FileSubmissionStore : ISubmissionStore
{
    private readonly string _root;
    private readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web) { WriteIndented = true };

    public FileSubmissionStore(string root)
    {
        _root = root;
        Directory.CreateDirectory(Path.Combine(_root, "submissions"));
    }

    public async Task SaveAsync(SubmissionRecordDto submission, CancellationToken ct = default)
    {
        var path = Path.Combine(_root, "submissions", submission.InstanceId.ToString() + ".json");
        submission.SubmittedAt = submission.SubmittedAt == default ? DateTime.UtcNow : submission.SubmittedAt;
        await File.WriteAllTextAsync(path, JsonSerializer.Serialize(submission, _jsonOptions), ct);
    }

    public async Task<SubmissionRecordDto?> GetAsync(Guid instanceId, CancellationToken ct = default)
    {
        var path = Path.Combine(_root, "submissions", instanceId.ToString() + ".json");
        if (!File.Exists(path)) return null;
        var json = await File.ReadAllTextAsync(path, ct);
        return JsonSerializer.Deserialize<SubmissionRecordDto>(json, _jsonOptions);
    }

    public async Task<bool> DeleteAsync(Guid instanceId, CancellationToken ct = default)
    {
        var path = Path.Combine(_root, "submissions", instanceId.ToString() + ".json");
        if (!File.Exists(path)) return false;
        await Task.Run(() => File.Delete(path), ct);
        return true;
    }

    public async Task<List<SubmissionRecordDto>> QueryAsync(string? formId = null, string? status = null, string? submittedBy = null, CancellationToken ct = default)
    {
        var folder = Path.Combine(_root, "submissions");
        var files = Directory.Exists(folder) ? Directory.GetFiles(folder, "*.json") : Array.Empty<string>();
        var list = new List<SubmissionRecordDto>();
        foreach (var f in files)
        {
            try
            {
                var json = await File.ReadAllTextAsync(f, ct);
                var s = JsonSerializer.Deserialize<SubmissionRecordDto>(json, _jsonOptions);
                if (s == null) continue;
                if (!string.IsNullOrEmpty(formId) && s.FormId != formId) continue;
                if (!string.IsNullOrEmpty(status) && s.Status != status) continue;
                if (!string.IsNullOrEmpty(submittedBy) && s.SubmittedBy != submittedBy) continue;
                list.Add(s);
            }
            catch { /* ignore */ }
        }
        return list.OrderByDescending(s => s.SubmittedAt).ToList();
    }
}

