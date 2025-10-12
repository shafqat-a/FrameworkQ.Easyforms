namespace FrameworkQ.Easyforms.Core.Interfaces;

using FrameworkQ.Easyforms.Core.Models;

/// <summary>
/// Interface for parsing HTML form templates into FormDefinition objects
/// </summary>
public interface IFormParser
{
    /// <summary>
    /// Parse HTML string into a FormDefinition model
    /// </summary>
    /// <param name="htmlContent">Raw HTML content</param>
    /// <returns>Parsed form definition</returns>
    Task<FormDefinition> ParseAsync(string htmlContent);

    /// <summary>
    /// Validate HTML structure and data-* attributes
    /// </summary>
    /// <param name="htmlContent">Raw HTML content</param>
    /// <returns>Validation result with errors if any</returns>
    Task<ValidationResult> ValidateAsync(string htmlContent);

    /// <summary>
    /// Sanitize HTML content using allowlist
    /// </summary>
    /// <param name="htmlContent">Raw HTML content</param>
    /// <returns>Sanitized HTML</returns>
    string Sanitize(string htmlContent);
}

public class ValidationResult
{
    public bool IsValid { get; set; }
    public List<string> Errors { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
}
