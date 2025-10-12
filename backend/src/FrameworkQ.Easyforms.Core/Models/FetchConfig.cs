namespace FrameworkQ.Easyforms.Core.Models;

/// <summary>
/// External data fetching configuration for fields
/// </summary>
public class FetchConfig
{
    public string Method { get; set; } = "GET";
    public string Url { get; set; } = string.Empty;
    public string FetchOn { get; set; } = "focus";
    public int MinChars { get; set; } = 0;
    public int DebounceMs { get; set; } = 300;
    public string? Map { get; set; }
    public string Cache { get; set; } = "session";
    public string[]? Depends { get; set; }
}
