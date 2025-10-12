namespace FrameworkQ.Easyforms.Core.Models;

/// <summary>
/// Table footer aggregate calculation
/// </summary>
public class Aggregate
{
    public string Function { get; set; } = string.Empty;
    public string? Column { get; set; }
    public string TargetId { get; set; } = string.Empty;
}
