namespace FrameworkQ.Easyforms.Core.Expressions;

/// <summary>
/// Tokenizer for expression strings
/// </summary>
public class Tokenizer
{
    private readonly string _input;
    private int _position;

    public Tokenizer(string input)
    {
        _input = input ?? throw new ArgumentNullException(nameof(input));
        _position = 0;
    }

    public List<Token> Tokenize()
    {
        var tokens = new List<Token>();

        while (_position < _input.Length)
        {
            SkipWhitespace();
            if (_position >= _input.Length) break;

            var ch = _input[_position];

            if (char.IsDigit(ch))
            {
                tokens.Add(ReadNumber());
            }
            else if (char.IsLetter(ch) || ch == '_')
            {
                tokens.Add(ReadIdentifier());
            }
            else if (ch == '"' || ch == '\'')
            {
                tokens.Add(ReadString(ch));
            }
            else
            {
                tokens.Add(ReadOperator());
            }
        }

        tokens.Add(new Token { Type = TokenType.EOF });
        return tokens;
    }

    private void SkipWhitespace()
    {
        while (_position < _input.Length && char.IsWhiteSpace(_input[_position]))
        {
            _position++;
        }
    }

    private Token ReadNumber()
    {
        var start = _position;
        while (_position < _input.Length && (char.IsDigit(_input[_position]) || _input[_position] == '.'))
        {
            _position++;
        }
        var value = _input.Substring(start, _position - start);
        return new Token { Type = TokenType.Number, Value = value };
    }

    private Token ReadIdentifier()
    {
        var start = _position;
        while (_position < _input.Length && (char.IsLetterOrDigit(_input[_position]) || _input[_position] == '_' || _input[_position] == '.'))
        {
            _position++;
        }
        var value = _input.Substring(start, _position - start);

        // Check for boolean literals
        if (value == "true" || value == "false")
        {
            return new Token { Type = TokenType.Boolean, Value = value };
        }

        return new Token { Type = TokenType.Identifier, Value = value };
    }

    private Token ReadString(char quote)
    {
        _position++; // Skip opening quote
        var start = _position;
        while (_position < _input.Length && _input[_position] != quote)
        {
            _position++;
        }
        var value = _input.Substring(start, _position - start);
        _position++; // Skip closing quote
        return new Token { Type = TokenType.String, Value = value };
    }

    private Token ReadOperator()
    {
        var ch = _input[_position];
        _position++;

        // Two-character operators
        if (_position < _input.Length)
        {
            var twoChar = $"{ch}{_input[_position]}";
            if (twoChar == "==" || twoChar == "!=" || twoChar == "<=" || twoChar == ">=" || twoChar == "&&" || twoChar == "||")
            {
                _position++;
                return new Token { Type = TokenType.Operator, Value = twoChar };
            }
        }

        // Single-character operators
        var type = ch switch
        {
            '(' => TokenType.LeftParen,
            ')' => TokenType.RightParen,
            ',' => TokenType.Comma,
            '+' or '-' or '*' or '/' or '%' or '<' or '>' or '!' => TokenType.Operator,
            _ => throw new InvalidOperationException($"Unknown character: {ch}")
        };

        return new Token { Type = type, Value = ch.ToString() };
    }
}

public class Token
{
    public TokenType Type { get; set; }
    public string Value { get; set; } = string.Empty;
}

public enum TokenType
{
    Number,
    String,
    Boolean,
    Identifier,
    Operator,
    LeftParen,
    RightParen,
    Comma,
    EOF
}
