namespace FrameworkQ.Easyforms.Core.Expressions;

/// <summary>
/// Expression evaluator - combines tokenizer and parser
/// </summary>
public class Evaluator
{
    /// <summary>
    /// Evaluate an expression string with given context
    /// </summary>
    /// <param name="expression">Expression string (e.g., "forced + scheduled")</param>
    /// <param name="context">Field values dictionary</param>
    /// <returns>Evaluation result</returns>
    public static object? Evaluate(string expression, Dictionary<string, object?> context)
    {
        if (string.IsNullOrWhiteSpace(expression))
        {
            return null;
        }

        try
        {
            // Tokenize
            var tokenizer = new Tokenizer(expression);
            var tokens = tokenizer.Tokenize();

            // Parse
            var parser = new Parser(tokens);
            var ast = parser.Parse();

            // Evaluate
            return ast.Evaluate(context);
        }
        catch (Exception ex)
        {
            throw new ExpressionEvaluationException($"Failed to evaluate expression '{expression}': {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Convert expression to SQL syntax
    /// </summary>
    /// <param name="expression">Expression string</param>
    /// <returns>SQL expression string</returns>
    public static string ToSql(string expression)
    {
        if (string.IsNullOrWhiteSpace(expression))
        {
            return "NULL";
        }

        try
        {
            // Tokenize
            var tokenizer = new Tokenizer(expression);
            var tokens = tokenizer.Tokenize();

            // Parse
            var parser = new Parser(tokens);
            var ast = parser.Parse();

            // Convert to SQL
            return ast.ToSql();
        }
        catch (Exception ex)
        {
            throw new ExpressionEvaluationException($"Failed to convert expression '{expression}' to SQL: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Validate expression syntax without evaluating
    /// </summary>
    /// <param name="expression">Expression string</param>
    /// <returns>True if valid, false otherwise</returns>
    public static bool Validate(string expression, out string? errorMessage)
    {
        errorMessage = null;

        if (string.IsNullOrWhiteSpace(expression))
        {
            errorMessage = "Expression cannot be empty";
            return false;
        }

        try
        {
            var tokenizer = new Tokenizer(expression);
            var tokens = tokenizer.Tokenize();
            var parser = new Parser(tokens);
            parser.Parse();
            return true;
        }
        catch (Exception ex)
        {
            errorMessage = ex.Message;
            return false;
        }
    }
}

public class ExpressionEvaluationException : Exception
{
    public ExpressionEvaluationException(string message) : base(message) { }
    public ExpressionEvaluationException(string message, Exception innerException) : base(message, innerException) { }
}
