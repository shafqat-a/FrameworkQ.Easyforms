/**
 * Copied from frontend/src/js/expression-evaluator.js for UI hosting
 */
(function(window) {
    'use strict';
    window.ExpressionEvaluator = {
        evaluate: function(expression, context) {
            if (!expression || expression.trim() === '') return null;
            var tokens = this._tokenize(expression); var ast = this._parse(tokens); return this._evaluateNode(ast, context);
        },
        _tokenize: function(input) {
            var tokens = [], pos = 0; while (pos < input.length) { while (pos < input.length && /\s/.test(input[pos])) pos++; if (pos >= input.length) break; var ch = input[pos];
                if (/\d/.test(ch)) { var start = pos; while (pos < input.length && /[\d.]/.test(input[pos])) pos++; tokens.push({ type:'NUMBER', value: input.substring(start, pos)}); }
                else if (/[a-zA-Z_]/.test(ch)) { var start2 = pos; while (pos < input.length && /[a-zA-Z0-9_.]/.test(input[pos])) pos++; var value = input.substring(start2, pos); tokens.push({ type: (value==='true'||value==='false') ? 'BOOLEAN' : 'IDENTIFIER', value: value}); }
                else if (ch === '"' || ch === "'") { var quote = ch; pos++; var start3 = pos; while (pos < input.length && input[pos] !== quote) pos++; tokens.push({ type:'STRING', value: input.substring(start3, pos)}); pos++; }
                else { if (pos + 1 < input.length) { var two = input.substring(pos, pos+2); if (['==','!=','<=','>=','&&','||'].indexOf(two)!==-1) { tokens.push({ type:'OPERATOR', value: two}); pos+=2; continue; } }
                    if ('()'.indexOf(ch)!==-1) tokens.push({ type: ch==='(' ? 'LPAREN':'RPAREN', value: ch });
                    else if (ch===',') tokens.push({ type:'COMMA', value: ch});
                    else if ('+-*/%<>!'.indexOf(ch)!==-1) tokens.push({ type:'OPERATOR', value: ch});
                    pos++; }
            }
            tokens.push({ type:'EOF', value:''}); return tokens;
        },
        _parse: function(tokens) {
            var pos = 0; function current(){ return tokens[pos]; } function advance(){ return tokens[pos++]; }
            function parseExpression(){ return parseLogicalOr(); }
            function parseLogicalOr(){ var left = parseLogicalAnd(); while (current().value==='||'){ advance(); left = { type:'BINARY', op:'||', left:left, right: parseLogicalAnd()}; } return left; }
            function parseLogicalAnd(){ var left = parseEquality(); while (current().value==='&&'){ advance(); left = { type:'BINARY', op:'&&', left:left, right: parseEquality()}; } return left; }
            function parseEquality(){ var left = parseRelational(); while (current().value==='=='||current().value==='!='){ var op = advance().value; left = { type:'BINARY', op:op, left:left, right: parseRelational()}; } return left; }
            function parseRelational(){ var left = parseAdditive(); while (['<','>','<=','>='].indexOf(current().value)!==-1){ var op = advance().value; left = { type:'BINARY', op:op, left:left, right: parseAdditive()}; } return left; }
            function parseAdditive(){ var left = parseMultiplicative(); while (current().value==='+'||current().value==='-'){ var op = advance().value; left = { type:'BINARY', op:op, left:left, right: parseMultiplicative()}; } return left; }
            function parseMultiplicative(){ var left = parseUnary(); while (['*','/','%'].indexOf(current().value)!==-1){ var op = advance().value; left = { type:'BINARY', op:op, left:left, right: parseUnary()}; } return left; }
            function parseUnary(){ if (current().value==='-'||current().value==='!'){ var op = advance().value; return { type:'UNARY', op:op, operand: parseUnary()}; } return parsePrimary(); }
            function parsePrimary(){ var token = current(); if (token.type==='NUMBER'){ advance(); return { type:'LITERAL', value: parseFloat(token.value)}; }
                if (token.type==='STRING'){ advance(); return { type:'LITERAL', value: token.value}; }
                if (token.type==='BOOLEAN'){ advance(); return { type:'LITERAL', value: token.value==='true'}; }
                if (token.type==='IDENTIFIER'){ var name = advance().value; if (current().type==='LPAREN'){ advance(); var args = []; if (current().type!=='RPAREN'){ args.push(parseExpression()); while (current().type==='COMMA'){ advance(); args.push(parseExpression()); } } if (current().type!=='RPAREN') throw new Error('Expected )'); advance(); return { type:'FUNCTION', name:name, args:args}; } return { type:'FIELD', name:name}; }
                if (token.type==='LPAREN'){ advance(); var expr = parseExpression(); if (current().type!=='RPAREN') throw new Error('Expected )'); advance(); return expr; }
                throw new Error('Unexpected token: ' + token.value);
            }
            return parseExpression();
        },
        _evaluateNode: function(node, context) {
            switch (node.type){
                case 'LITERAL': return node.value;
                case 'FIELD': var name = node.name; if (name.startsWith('ctx.')) name = name.substring(4); return context[name] !== undefined ? context[name] : null;
                case 'BINARY': var l = this._evaluateNode(node.left, context), r = this._evaluateNode(node.right, context); return this._evaluateBinary(node.op, l, r);
                case 'UNARY': var v = this._evaluateNode(node.operand, context); return node.op==='-' ? -v : !v;
                case 'FUNCTION': var args = node.args.map(a=>this._evaluateNode(a, context)); return this._evaluateFunction(node.name, args);
                default: throw new Error('Unknown node type: ' + node.type);
            }
        },
        _evaluateBinary: function(op, left, right) {
            if (left===null || right===null){ if (op==='=='||op==='!=') return op==='==' ? left===right : left!==right; return null; }
            switch (op){ case '+': return left+right; case '-': return left-right; case '*': return left*right; case '/': return right!==0? left/right : null; case '%': return left%right; case '==': return left===right; case '!=': return left!==right; case '<': return left<right; case '>': return left>right; case '<=': return left<=right; case '>=': return left>=right; case '&&': return left && right; case '||': return left || right; default: throw new Error('Unknown op: '+op);} 
        },
        _evaluateFunction: function(name, args) {
            name = name.toLowerCase();
            switch (name){
                case 'round': return args.length>=1 ? Math.round(args[0]*Math.pow(10, args[1]||0))/Math.pow(10, args[1]||0) : null;
                case 'abs': return args.length>=1 ? Math.abs(args[0]) : null;
                case 'ceil': return args.length>=1 ? Math.ceil(args[0]) : null;
                case 'floor': return args.length>=1 ? Math.floor(args[0]) : null;
                case 'min': return args.length>0 ? Math.min(...args.filter(a=>a!==null)) : null;
                case 'max': return args.length>0 ? Math.max(...args.filter(a=>a!==null)) : null;
                case 'sum': return args.filter(a=>a!==null).reduce((a,b)=>a+b,0);
                case 'avg': var vals = args.filter(a=>a!==null); return vals.length>0 ? vals.reduce((a,b)=>a+b,0)/vals.length : null;
                case 'count': return args.filter(a=>a!==null).length;
                default: throw new Error('Unknown function: ' + name);
            }
        }
    };
})(window);

