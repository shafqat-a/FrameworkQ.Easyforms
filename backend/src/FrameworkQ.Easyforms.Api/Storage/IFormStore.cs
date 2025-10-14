namespace FrameworkQ.Easyforms.Api.Storage;

public class FormMeta
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Version { get; set; } = "1.0";
    public string[] Tags { get; set; } = Array.Empty<string>();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

public interface IFormStore
{
    Task SaveAsync(string id, string html, string schemaJson, FormMeta meta, CancellationToken ct = default);
    Task<FormMeta?> GetMetaAsync(string id, CancellationToken ct = default);
    Task<string?> GetHtmlAsync(string id, CancellationToken ct = default);
    Task<string?> GetSchemaAsync(string id, CancellationToken ct = default);
    Task<List<FormMeta>> ListAsync(CancellationToken ct = default);
}

