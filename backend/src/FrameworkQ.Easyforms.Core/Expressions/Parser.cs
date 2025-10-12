namespace FrameworkQ.Easyforms.Core.Expressions;

/// <summary>
/// Recursive descent parser for expressions
/// Grammar:
/// expression := logical_or
/// logical_or := logical_and ('||' logical_and)*
/// logical_and := equality ('&&' equality)*
/// equality := relational (('==' | '!=') relational)*
/// relational := additive (('<' | '>' | '<=' | '>=') additive)*
/// additive := multiplicative (('+' | '-') multiplicative)*
/// multiplicative := unary (('*' | '/' | '%') unary)*
/// unary := ('!' | '-') unary | primary
/// primary := number | string | boolean | identifier | function_call | '(' expression ')'
/// function_call := identifier '(' (expression (',' expression)*)? ')'
/// </summary>
public class Parser
{
    private readonly List<Token> _tokens;
    private int _position;

    public Parser(List<Token> tokens)
    {
        _tokens = tokens;
        _position = 0;
    }

    public ExpressionNode Parse()
    {
        var expr = ParseExpression();
        if (Current().Type != TokenType.EOF)
        {
            throw new InvalidOperationException($"Unexpected token: {Current().Value}");
        }
        return expr;
    }

    private ExpressionNode ParseExpression()
    {
        return ParseLogicalOr();
    }

    private ExpressionNode ParseLogicalOr()
    {
        var left = ParseLogicalAnd();
        while (Match("||"))
        {
            var op = Previous().Value;
            var right = ParseLogicalAnd();
            left = new BinaryOp(left, op, right);
        }
        return left;
    }

    private ExpressionNode ParseLogicalAnd()
    {
        var left = ParseEquality();
        while (Match("&&"))
        {
            var op = Previous().Value;
            var right = ParseEquality();
            left = new BinaryOp(left, op, right);
        }
        return left;
    }

    private ExpressionNode ParseEquality()
    {
        var left = ParseRelational();
        while (Match("==", "!="))
        {
            var op = Previous().Value;
            var right = ParseRelational();
            left = new BinaryOp(left, op, right);
        }
        return left;
    }

    private ExpressionNode ParseRelational()
    {
        var left = ParseAdditive();
        while (Match("<", ">", "<=", ">="))
        {
            var op = Previous().Value;
            var right = ParseAdditive();
            left = new BinaryOp(left, op, right);
        }
        return left;
    }

    private ExpressionNode ParseAdditive()
    {
        var left = ParseMultiplicative();
        while (Match("+", "-"))
        {
            var op = Previous().Value;
            var right = ParseMultiplicative();
            left = new BinaryOp(left, op, right);
        }
        return left;
    }

    private ExpressionNode ParseMultiplicative()
    {
        var left = ParseUnary();
        while (Match("*", "/", "%"))
        {
            var op = Previous().Value;
            var right = ParseUnary();
            left = new BinaryOp(left, op, right);
        }
        return left;
    }

    private ExpressionNode ParseUnary()
    {
        if (Match("!", "-"))
        {
            var op = Previous().Value;
            var right = ParseUnary();
            // Unary operators handled as special binary ops with 0
            if (op == "-")
            {
                return new BinaryOp(new LiteralNode(0), "-", right);
            }
            // Logical NOT
            return new BinaryOp(right, "==", new LiteralNode(false));
        }
        return ParsePrimary();
    }

    private ExpressionNode ParsePrimary()
    {
        // Number
        if (Current().Type == TokenType.Number)
        {
            var value = double.Parse(Advance().Value);
            return new LiteralNode(value);
        }

        // String
        if (Current().Type == TokenType.String)
        {
            var value = Advance().Value;
            return new LiteralNode(value);
        }

        // Boolean
        if (Current().Type == TokenType.Boolean)
        {
            var value = bool.Parse(Advance().Value);
            return new LiteralNode(value);
        }

        // Function call or identifier
        if (Current().Type == TokenType.Identifier)
        {
            var identifier = Advance().Value;

            // Check for function call
            if (Current().Type == TokenType.LeftParen)
            {
                Advance(); // Consume '('
                var args = new List<ExpressionNode>();

                if (Current().Type != TokenType.RightParen)
                {
                    args.Add(ParseExpression());
                    while (Current().Type == TokenType.Comma)
                    {
                        Advance(); // Consume ','
                        args.Add(ParseExpression());
                    }
                }

                Consume(TokenType.RightParen, "Expected ')' after function arguments");
                return new FunctionCall(identifier, args);
            }

            // Field reference
            return new FieldRef(identifier);
        }

        // Grouped expression
        if (Current().Type == TokenType.LeftParen)
        {
            Advance(); // Consume '('
            var expr = ParseExpression();
            Consume(TokenType.RightParen, "Expected ')' after expression");
            return expr;
        }

        throw new InvalidOperationException($"Unexpected token: {Current().Value}");
    }

    private Token Current() => _tokens[_position];
    private Token Previous() => _tokens[_position - 1];

    private Token Advance()
    {
        if (_position < _tokens.Count - 1) _position++;
        return Previous();
    }

    private bool Match(params string[] values)
    {
        if (Current().Type == TokenType.Operator && values.Contains(Current().Value))
        {
            Advance();
            return true;
        }
        return false;
    }

    private void Consume(TokenType type, string message)
    {
        if (Current().Type == type)
        {
            Advance();
            return;
        }
        throw new InvalidOperationException(message);
    }
}
