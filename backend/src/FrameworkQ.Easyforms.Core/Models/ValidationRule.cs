namespace FrameworkQ.Easyforms.Core.Models;

/// <summary>
/// Validation constraint for a field or container
/// </summary>
public class ValidationRule
{
    public string Type { get; set; } = string.Empty;
    public string? Expression { get; set; }
    public string? Message { get; set; }
    public string Severity { get; set; } = "error";
    public string ValidateOn { get; set; } = "change";
}
