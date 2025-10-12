/**
 * FrameworkQ.Easyforms - Expression Evaluator
 * Client-side formula and conditional logic evaluation
 * Mirrors the C# expression engine for consistency
 */

(function(window) {
    'use strict';

    /**
     * Expression Evaluator
     */
    window.ExpressionEvaluator = {
        /**
         * Evaluate an expression with given context
         * @param {string} expression - Expression string (e.g., "forced + scheduled")
         * @param {Object} context - Field values object
         * @returns {*} Evaluation result
         */
        evaluate: function(expression, context) {
            if (!expression || expression.trim() === '') {
                return null;
            }

            try {
                var tokens = this._tokenize(expression);
                var ast = this._parse(tokens);
                return this._evaluateNode(ast, context);
            } catch (e) {
                console.error('[ExpressionEvaluator] Failed to evaluate:', expression, e);
                throw e;
            }
        },

        /**
         * Tokenize expression string
         * @private
         */
        _tokenize: function(input) {
            var tokens = [];
            var pos = 0;

            while (pos < input.length) {
                // Skip whitespace
                while (pos < input.length && /\s/.test(input[pos])) {
                    pos++;
                }

                if (pos >= input.length) break;

                var ch = input[pos];

                // Number
                if (/\d/.test(ch)) {
                    var start = pos;
                    while (pos < input.length && /[\d.]/.test(input[pos])) {
                        pos++;
                    }
                    tokens.push({ type: 'NUMBER', value: input.substring(start, pos) });
                }
                // Identifier or function
                else if (/[a-zA-Z_]/.test(ch)) {
                    var start = pos;
                    while (pos < input.length && /[a-zA-Z0-9_.]/.test(input[pos])) {
                        pos++;
                    }
                    var value = input.substring(start, pos);
                    tokens.push({ type: value === 'true' || value === 'false' ? 'BOOLEAN' : 'IDENTIFIER', value: value });
                }
                // String (single or double quotes)
                else if (ch === '"' || ch === "'") {
                    var quote = ch;
                    pos++; // Skip opening quote
                    var start = pos;
                    while (pos < input.length && input[pos] !== quote) {
                        pos++;
                    }
                    tokens.push({ type: 'STRING', value: input.substring(start, pos) });
                    pos++; // Skip closing quote
                }
                // Operators
                else {
                    // Two-char operators
                    if (pos + 1 < input.length) {
                        var twoChar = input.substring(pos, pos + 2);
                        if (['==', '!=', '<=', '>=', '&&', '||'].indexOf(twoChar) !== -1) {
                            tokens.push({ type: 'OPERATOR', value: twoChar });
                            pos += 2;
                            continue;
                        }
                    }

                    // Single-char
                    if ('()'.indexOf(ch) !== -1) {
                        tokens.push({ type: ch === '(' ? 'LPAREN' : 'RPAREN', value: ch });
                    } else if (ch === ',') {
                        tokens.push({ type: 'COMMA', value: ch });
                    } else if ('+-*/%<>!'.indexOf(ch) !== -1) {
                        tokens.push({ type: 'OPERATOR', value: ch });
                    }
                    pos++;
                }
            }

            tokens.push({ type: 'EOF', value: '' });
            return tokens;
        },

        /**
         * Parse tokens into AST
         * @private
         */
        _parse: function(tokens) {
            var pos = 0;

            function current() { return tokens[pos]; }
            function advance() { return tokens[pos++]; }

            function parseExpression() {
                return parseLogicalOr();
            }

            function parseLogicalOr() {
                var left = parseLogicalAnd();
                while (current().value === '||') {
                    advance();
                    left = { type: 'BINARY', op: '||', left: left, right: parseLogicalAnd() };
                }
                return left;
            }

            function parseLogicalAnd() {
                var left = parseEquality();
                while (current().value === '&&') {
                    advance();
                    left = { type: 'BINARY', op: '&&', left: left, right: parseEquality() };
                }
                return left;
            }

            function parseEquality() {
                var left = parseRelational();
                while (current().value === '==' || current().value === '!=') {
                    var op = advance().value;
                    left = { type: 'BINARY', op: op, left: left, right: parseRelational() };
                }
                return left;
            }

            function parseRelational() {
                var left = parseAdditive();
                while (['<', '>', '<=', '>='].indexOf(current().value) !== -1) {
                    var op = advance().value;
                    left = { type: 'BINARY', op: op, left: left, right: parseAdditive() };
                }
                return left;
            }

            function parseAdditive() {
                var left = parseMultiplicative();
                while (current().value === '+' || current().value === '-') {
                    var op = advance().value;
                    left = { type: 'BINARY', op: op, left: left, right: parseMultiplicative() };
                }
                return left;
            }

            function parseMultiplicative() {
                var left = parseUnary();
                while (['*', '/', '%'].indexOf(current().value) !== -1) {
                    var op = advance().value;
                    left = { type: 'BINARY', op: op, left: left, right: parseUnary() };
                }
                return left;
            }

            function parseUnary() {
                if (current().value === '-' || current().value === '!') {
                    var op = advance().value;
                    return { type: 'UNARY', op: op, operand: parseUnary() };
                }
                return parsePrimary();
            }

            function parsePrimary() {
                var token = current();

                // Number
                if (token.type === 'NUMBER') {
                    advance();
                    return { type: 'LITERAL', value: parseFloat(token.value) };
                }

                // String
                if (token.type === 'STRING') {
                    advance();
                    return { type: 'LITERAL', value: token.value };
                }

                // Boolean
                if (token.type === 'BOOLEAN') {
                    advance();
                    return { type: 'LITERAL', value: token.value === 'true' };
                }

                // Identifier or function
                if (token.type === 'IDENTIFIER') {
                    var name = advance().value;

                    // Function call
                    if (current().type === 'LPAREN') {
                        advance(); // consume '('
                        var args = [];

                        if (current().type !== 'RPAREN') {
                            args.push(parseExpression());
                            while (current().type === 'COMMA') {
                                advance(); // consume ','
                                args.push(parseExpression());
                            }
                        }

                        if (current().type !== 'RPAREN') {
                            throw new Error('Expected ) after function arguments');
                        }
                        advance(); // consume ')'

                        return { type: 'FUNCTION', name: name, args: args };
                    }

                    // Field reference
                    return { type: 'FIELD', name: name };
                }

                // Grouped expression
                if (token.type === 'LPAREN') {
                    advance(); // consume '('
                    var expr = parseExpression();
                    if (current().type !== 'RPAREN') {
                        throw new Error('Expected ) after expression');
                    }
                    advance(); // consume ')'
                    return expr;
                }

                throw new Error('Unexpected token: ' + token.value);
            }

            return parseExpression();
        },

        /**
         * Evaluate AST node
         * @private
         */
        _evaluateNode: function(node, context) {
            switch (node.type) {
                case 'LITERAL':
                    return node.value;

                case 'FIELD':
                    var fieldName = node.name;
                    // Handle ctx. prefix for global context
                    if (fieldName.startsWith('ctx.')) {
                        fieldName = fieldName.substring(4);
                    }
                    return context[fieldName] !== undefined ? context[fieldName] : null;

                case 'BINARY':
                    var left = this._evaluateNode(node.left, context);
                    var right = this._evaluateNode(node.right, context);
                    return this._evaluateBinary(node.op, left, right);

                case 'UNARY':
                    var operand = this._evaluateNode(node.operand, context);
                    return node.op === '-' ? -operand : !operand;

                case 'FUNCTION':
                    var args = node.args.map(arg => this._evaluateNode(arg, context));
                    return this._evaluateFunction(node.name, args);

                default:
                    throw new Error('Unknown node type: ' + node.type);
            }
        },

        /**
         * Evaluate binary operation
         * @private
         */
        _evaluateBinary: function(op, left, right) {
            // Handle null values
            if (left === null || right === null) {
                if (op === '==' || op === '!=') {
                    return op === '==' ? left === right : left !== right;
                }
                return null;
            }

            switch (op) {
                case '+': return left + right;
                case '-': return left - right;
                case '*': return left * right;
                case '/': return right !== 0 ? left / right : null;
                case '%': return left % right;
                case '==': return left === right;
                case '!=': return left !== right;
                case '<': return left < right;
                case '>': return left > right;
                case '<=': return left <= right;
                case '>=': return left >= right;
                case '&&': return left && right;
                case '||': return left || right;
                default: throw new Error('Unknown operator: ' + op);
            }
        },

        /**
         * Evaluate function call
         * @private
         */
        _evaluateFunction: function(name, args) {
            name = name.toLowerCase();

            switch (name) {
                case 'round':
                    return args.length >= 1 ? Math.round(args[0] * Math.pow(10, args[1] || 0)) / Math.pow(10, args[1] || 0) : null;
                case 'abs':
                    return args.length >= 1 ? Math.abs(args[0]) : null;
                case 'ceil':
                    return args.length >= 1 ? Math.ceil(args[0]) : null;
                case 'floor':
                    return args.length >= 1 ? Math.floor(args[0]) : null;
                case 'min':
                    return args.length > 0 ? Math.min(...args.filter(a => a !== null)) : null;
                case 'max':
                    return args.length > 0 ? Math.max(...args.filter(a => a !== null)) : null;
                case 'sum':
                    return args.filter(a => a !== null).reduce((a, b) => a + b, 0);
                case 'avg':
                    var values = args.filter(a => a !== null);
                    return values.length > 0 ? values.reduce((a, b) => a + b, 0) / values.length : null;
                case 'count':
                    return args.filter(a => a !== null).length;
                default:
                    throw new Error('Unknown function: ' + name);
            }
        }
    };

})(window);
