using System.Text.Json;

namespace FrameworkQ.Easyforms.Api.Storage;

public class FileFormStore : IFormStore
{
    private readonly string _root;
    private readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web) { WriteIndented = true };

    public FileFormStore(string root)
    {
        _root = root;
        Directory.CreateDirectory(Path.Combine(_root, "forms"));
    }

    public async Task SaveAsync(string id, string html, string schemaJson, FormMeta meta, CancellationToken ct = default)
    {
        var dir = Path.Combine(_root, "forms", Sanitize(id));
        Directory.CreateDirectory(dir);

        meta.Id = id;
        meta.UpdatedAt = DateTime.UtcNow;
        if (meta.CreatedAt == default) meta.CreatedAt = DateTime.UtcNow;

        await File.WriteAllTextAsync(Path.Combine(dir, "form.html"), html, ct);
        await File.WriteAllTextAsync(Path.Combine(dir, "schema.json"), schemaJson, ct);
        await File.WriteAllTextAsync(Path.Combine(dir, "meta.json"), JsonSerializer.Serialize(meta, _jsonOptions), ct);
    }

    public async Task<FormMeta?> GetMetaAsync(string id, CancellationToken ct = default)
    {
        var path = Path.Combine(_root, "forms", Sanitize(id), "meta.json");
        if (!File.Exists(path)) return null;
        var json = await File.ReadAllTextAsync(path, ct);
        return JsonSerializer.Deserialize<FormMeta>(json, _jsonOptions);
    }

    public async Task<string?> GetHtmlAsync(string id, CancellationToken ct = default)
    {
        var path = Path.Combine(_root, "forms", Sanitize(id), "form.html");
        if (!File.Exists(path)) return null;
        return await File.ReadAllTextAsync(path, ct);
    }

    public async Task<string?> GetSchemaAsync(string id, CancellationToken ct = default)
    {
        var path = Path.Combine(_root, "forms", Sanitize(id), "schema.json");
        if (!File.Exists(path)) return null;
        return await File.ReadAllTextAsync(path, ct);
    }

    public async Task<List<FormMeta>> ListAsync(CancellationToken ct = default)
    {
        var folder = Path.Combine(_root, "forms");
        var dirs = Directory.Exists(folder) ? Directory.GetDirectories(folder) : Array.Empty<string>();
        var list = new List<FormMeta>();
        foreach (var d in dirs)
        {
            var meta = await TryReadMetaAsync(Path.Combine(d, "meta.json"), ct);
            if (meta != null) list.Add(meta);
        }
        return list.OrderByDescending(m => m.UpdatedAt).ToList();
    }

    private async Task<FormMeta?> TryReadMetaAsync(string path, CancellationToken ct)
    {
        try
        {
            if (!File.Exists(path)) return null;
            var json = await File.ReadAllTextAsync(path, ct);
            return JsonSerializer.Deserialize<FormMeta>(json, _jsonOptions);
        }
        catch
        {
            return null;
        }
    }

    private static string Sanitize(string id)
    {
        foreach (var c in Path.GetInvalidFileNameChars()) id = id.Replace(c, '_');
        return id;
    }
}

