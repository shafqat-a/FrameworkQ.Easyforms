namespace FrameworkQ.Easyforms.Core.Expressions;

/// <summary>
/// Function call node (sum, avg, min, max, count, round, abs, ceil, floor)
/// </summary>
public class FunctionCall : ExpressionNode
{
    public string FunctionName { get; set; }
    public List<ExpressionNode> Arguments { get; set; }

    public FunctionCall(string functionName, List<ExpressionNode> arguments)
    {
        FunctionName = functionName.ToLower();
        Arguments = arguments;
    }

    public override object? Evaluate(Dictionary<string, object?> context)
    {
        var args = Arguments.Select(arg => arg.Evaluate(context)).ToList();

        return FunctionName switch
        {
            "round" => Round(args),
            "abs" => Abs(args),
            "ceil" => Ceil(args),
            "floor" => Floor(args),
            "min" => Min(args),
            "max" => Max(args),
            "sum" => Sum(args),
            "avg" => Avg(args),
            "count" => Count(args),
            _ => throw new InvalidOperationException($"Unknown function: {FunctionName}")
        };
    }

    public override string ToSql()
    {
        var argsSql = string.Join(", ", Arguments.Select(arg => arg.ToSql()));
        return $"{FunctionName.ToUpper()}({argsSql})";
    }

    private static object? Round(List<object?> args)
    {
        if (args.Count == 0 || args[0] is null) return null;
        var value = Convert.ToDouble(args[0]);
        var decimals = args.Count > 1 && args[1] != null ? Convert.ToInt32(args[1]) : 0;
        return Math.Round(value, decimals);
    }

    private static object? Abs(List<object?> args)
    {
        if (args.Count == 0 || args[0] is null) return null;
        return Math.Abs(Convert.ToDouble(args[0]));
    }

    private static object? Ceil(List<object?> args)
    {
        if (args.Count == 0 || args[0] is null) return null;
        return Math.Ceiling(Convert.ToDouble(args[0]));
    }

    private static object? Floor(List<object?> args)
    {
        if (args.Count == 0 || args[0] is null) return null;
        return Math.Floor(Convert.ToDouble(args[0]));
    }

    private static object? Min(List<object?> args)
    {
        var values = args.Where(a => a != null).Select(a => Convert.ToDouble(a)).ToList();
        return values.Any() ? values.Min() : null;
    }

    private static object? Max(List<object?> args)
    {
        var values = args.Where(a => a != null).Select(a => Convert.ToDouble(a)).ToList();
        return values.Any() ? values.Max() : null;
    }

    private static object? Sum(List<object?> args)
    {
        var values = args.Where(a => a != null).Select(a => Convert.ToDouble(a)).ToList();
        return values.Any() ? values.Sum() : 0.0;
    }

    private static object? Avg(List<object?> args)
    {
        var values = args.Where(a => a != null).Select(a => Convert.ToDouble(a)).ToList();
        return values.Any() ? values.Average() : null;
    }

    private static object? Count(List<object?> args)
    {
        return args.Count(a => a != null);
    }
}
