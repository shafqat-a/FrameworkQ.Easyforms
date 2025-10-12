namespace FrameworkQ.Easyforms.Core.Expressions;

/// <summary>
/// Field reference node (e.g., "total", "ctx.substation")
/// </summary>
public class FieldRef : ExpressionNode
{
    public string FieldName { get; set; }
    public bool IsGlobalContext { get; set; }

    public FieldRef(string fieldName)
    {
        if (fieldName.StartsWith("ctx."))
        {
            IsGlobalContext = true;
            FieldName = fieldName.Substring(4);
        }
        else
        {
            IsGlobalContext = false;
            FieldName = fieldName;
        }
    }

    public override object? Evaluate(Dictionary<string, object?> context)
    {
        var contextKey = IsGlobalContext ? $"ctx.{FieldName}" : FieldName;

        if (context.TryGetValue(contextKey, out var value))
        {
            return value;
        }

        // Try alternate key
        if (context.TryGetValue(FieldName, out var altValue))
        {
            return altValue;
        }

        return null;
    }

    public override string ToSql()
    {
        // In SQL context, field names should be properly escaped
        return $"[{FieldName}]";
    }
}
