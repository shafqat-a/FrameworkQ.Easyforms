namespace FrameworkQ.Easyforms.Core.Expressions;

/// <summary>
/// Binary operation node (+, -, *, /, ==, !=, <, >, &&, ||, etc.)
/// </summary>
public class BinaryOp : ExpressionNode
{
    public ExpressionNode Left { get; set; }
    public string Operator { get; set; }
    public ExpressionNode Right { get; set; }

    public BinaryOp(ExpressionNode left, string op, ExpressionNode right)
    {
        Left = left;
        Operator = op;
        Right = right;
    }

    public override object? Evaluate(Dictionary<string, object?> context)
    {
        var leftVal = Left.Evaluate(context);
        var rightVal = Right.Evaluate(context);

        return Operator switch
        {
            "+" => Add(leftVal, rightVal),
            "-" => Subtract(leftVal, rightVal),
            "*" => Multiply(leftVal, rightVal),
            "/" => Divide(leftVal, rightVal),
            "%" => Modulo(leftVal, rightVal),
            "==" => Equals(leftVal, rightVal),
            "!=" => !Equals(leftVal, rightVal),
            "<" => LessThan(leftVal, rightVal),
            ">" => GreaterThan(leftVal, rightVal),
            "<=" => LessThanOrEqual(leftVal, rightVal),
            ">=" => GreaterThanOrEqual(leftVal, rightVal),
            "&&" => And(leftVal, rightVal),
            "||" => Or(leftVal, rightVal),
            _ => throw new InvalidOperationException($"Unknown operator: {Operator}")
        };
    }

    public override string ToSql()
    {
        var sqlOp = Operator switch
        {
            "==" => "=",
            "!=" => "<>",
            "&&" => "AND",
            "||" => "OR",
            _ => Operator
        };
        return $"({Left.ToSql()} {sqlOp} {Right.ToSql()})";
    }

    private static object? Add(object? left, object? right)
    {
        if (left is null || right is null) return null;
        return Convert.ToDouble(left) + Convert.ToDouble(right);
    }

    private static object? Subtract(object? left, object? right)
    {
        if (left is null || right is null) return null;
        return Convert.ToDouble(left) - Convert.ToDouble(right);
    }

    private static object? Multiply(object? left, object? right)
    {
        if (left is null || right is null) return null;
        return Convert.ToDouble(left) * Convert.ToDouble(right);
    }

    private static object? Divide(object? left, object? right)
    {
        if (left is null || right is null) return null;
        var divisor = Convert.ToDouble(right);
        if (Math.Abs(divisor) < double.Epsilon) return null;
        return Convert.ToDouble(left) / divisor;
    }

    private static object? Modulo(object? left, object? right)
    {
        if (left is null || right is null) return null;
        return Convert.ToDouble(left) % Convert.ToDouble(right);
    }

    private static bool LessThan(object? left, object? right)
    {
        if (left is null || right is null) return false;
        return Convert.ToDouble(left) < Convert.ToDouble(right);
    }

    private static bool GreaterThan(object? left, object? right)
    {
        if (left is null || right is null) return false;
        return Convert.ToDouble(left) > Convert.ToDouble(right);
    }

    private static bool LessThanOrEqual(object? left, object? right)
    {
        if (left is null || right is null) return false;
        return Convert.ToDouble(left) <= Convert.ToDouble(right);
    }

    private static bool GreaterThanOrEqual(object? left, object? right)
    {
        if (left is null || right is null) return false;
        return Convert.ToDouble(left) >= Convert.ToDouble(right);
    }

    private static bool And(object? left, object? right)
    {
        return Convert.ToBoolean(left) && Convert.ToBoolean(right);
    }

    private static bool Or(object? left, object? right)
    {
        return Convert.ToBoolean(left) || Convert.ToBoolean(right);
    }
}
