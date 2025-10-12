namespace FrameworkQ.Easyforms.Core.Expressions;

/// <summary>
/// Base class for expression AST nodes
/// </summary>
public abstract class ExpressionNode
{
    public abstract object? Evaluate(Dictionary<string, object?> context);
    public abstract string ToSql();
}

/// <summary>
/// Literal value node (number, string, boolean)
/// </summary>
public class LiteralNode : ExpressionNode
{
    public object? Value { get; set; }

    public LiteralNode(object? value)
    {
        Value = value;
    }

    public override object? Evaluate(Dictionary<string, object?> context)
    {
        return Value;
    }

    public override string ToSql()
    {
        if (Value == null) return "NULL";
        if (Value is string s) return $"'{s.Replace("'", "''")}'";
        if (Value is bool b) return b ? "1" : "0";
        return Value.ToString() ?? "NULL";
    }
}
