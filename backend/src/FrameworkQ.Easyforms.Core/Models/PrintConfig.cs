namespace FrameworkQ.Easyforms.Core.Models;

/// <summary>
/// Print-specific configuration
/// </summary>
public class PrintConfig
{
    public string PageSize { get; set; } = "A4";
    public string Orientation { get; set; } = "portrait";
    public int[] MarginsM { get; set; } = new[] { 10, 10, 10, 10 };
    public double Scale { get; set; } = 1.0;
    public int RepeatHeadRows { get; set; } = 0;
    public string? PageBreak { get; set; }
    public bool KeepTogether { get; set; }
}
