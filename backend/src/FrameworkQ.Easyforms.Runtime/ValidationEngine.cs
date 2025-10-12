namespace FrameworkQ.Easyforms.Runtime;

using FrameworkQ.Easyforms.Core.Expressions;
using FrameworkQ.Easyforms.Core.Models;
using System.Text.RegularExpressions;

/// <summary>
/// Server-side validation engine
/// Validates form submissions against schema rules
/// </summary>
public class ValidationEngine
{
    /// <summary>
    /// Validate form submission data against form definition
    /// </summary>
    /// <param name="formDefinition">Form definition with validation rules</param>
    /// <param name="submissionData">Submitted form data</param>
    /// <returns>Validation result with errors</returns>
    public ValidationResult Validate(FormDefinition formDefinition, Dictionary<string, object?> submissionData)
    {
        var errors = new Dictionary<string, List<string>>();

        // Validate each page
        foreach (var page in formDefinition.Pages)
        {
            foreach (var section in page.Sections)
            {
                foreach (var widget in section.Widgets)
                {
                    ValidateWidget(widget, submissionData, errors);
                }
            }
        }

        return new ValidationResult
        {
            IsValid = errors.Count == 0,
            Errors = errors.Select(kvp => kvp.Key).ToList(),
            FieldErrors = errors
        };
    }

    private void ValidateWidget(Widget widget, Dictionary<string, object?> submissionData, Dictionary<string, List<string>> errors)
    {
        switch (widget)
        {
            case Field field:
                ValidateField(field, submissionData, errors);
                break;

            case Core.Models.Group group:
                foreach (var field in group.Fields)
                {
                    ValidateField(field, submissionData, errors);
                }
                break;

            case Table table:
                ValidateTable(table, submissionData, errors);
                break;

            // Other widget types validated as needed
        }
    }

    private void ValidateField(Field field, Dictionary<string, object?> submissionData, Dictionary<string, List<string>> errors)
    {
        var fieldErrors = new List<string>();
        var value = submissionData.ContainsKey(field.Name) ? submissionData[field.Name] : null;

        // Required validation
        if (field.Required && (value == null || string.IsNullOrWhiteSpace(value?.ToString())))
        {
            fieldErrors.Add(GetErrorMessage(field.ValidationRules, "required", "This field is required"));
        }

        // Skip other validations if value is empty and not required
        if (value == null || string.IsNullOrWhiteSpace(value?.ToString()))
        {
            if (fieldErrors.Count > 0)
            {
                errors[field.Name] = fieldErrors;
            }
            return;
        }

        var stringValue = value.ToString() ?? string.Empty;

        // Pattern validation
        if (!string.IsNullOrEmpty(field.Pattern))
        {
            try
            {
                var regex = new Regex(field.Pattern);
                if (!regex.IsMatch(stringValue))
                {
                    fieldErrors.Add(GetErrorMessage(field.ValidationRules, "pattern", "Value does not match required pattern"));
                }
            }
            catch (Exception ex)
            {
                fieldErrors.Add($"Invalid pattern: {ex.Message}");
            }
        }

        // Min/Max validation for numbers
        if (field.DataType == "integer" || field.DataType == "decimal")
        {
            if (double.TryParse(stringValue, out var numValue))
            {
                if (!string.IsNullOrEmpty(field.Min) && double.TryParse(field.Min, out var min))
                {
                    if (numValue < min)
                    {
                        fieldErrors.Add(GetErrorMessage(field.ValidationRules, "min", $"Value must be at least {min}"));
                    }
                }

                if (!string.IsNullOrEmpty(field.Max) && double.TryParse(field.Max, out var max))
                {
                    if (numValue > max)
                    {
                        fieldErrors.Add(GetErrorMessage(field.ValidationRules, "max", $"Value must be at most {max}"));
                    }
                }
            }
        }

        // Enum validation
        if (field.EnumValues != null && field.EnumValues.Length > 0)
        {
            if (!field.EnumValues.Contains(stringValue))
            {
                fieldErrors.Add($"Value must be one of: {string.Join(", ", field.EnumValues)}");
            }
        }

        // Conditional required (data-required-when)
        var conditionalRequiredRule = field.ValidationRules.FirstOrDefault(r => r.Type == "required-when");
        if (conditionalRequiredRule != null && !string.IsNullOrEmpty(conditionalRequiredRule.Expression))
        {
            try
            {
                var isRequired = Evaluator.Evaluate(conditionalRequiredRule.Expression, submissionData);
                if (Convert.ToBoolean(isRequired) && string.IsNullOrWhiteSpace(stringValue))
                {
                    fieldErrors.Add(conditionalRequiredRule.Message ?? "This field is required");
                }
            }
            catch (Exception ex)
            {
                fieldErrors.Add($"Failed to evaluate required-when condition: {ex.Message}");
            }
        }

        if (fieldErrors.Count > 0)
        {
            errors[field.Name] = fieldErrors;
        }
    }

    private void ValidateTable(Table table, Dictionary<string, object?> submissionData, Dictionary<string, List<string>> errors)
    {
        // TODO: Implement table row validation
        // For now, skip table validation (will be enhanced in future)
    }

    private string GetErrorMessage(List<ValidationRule> rules, string ruleType, string defaultMessage)
    {
        var rule = rules.FirstOrDefault(r => r.Type == ruleType);
        return rule?.Message ?? defaultMessage;
    }
}

public class ValidationResult
{
    public bool IsValid { get; set; }
    public List<string> Errors { get; set; } = new();
    public Dictionary<string, List<string>> FieldErrors { get; set; } = new();
}
