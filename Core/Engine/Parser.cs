using Calculator.Core.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Calculator.Core.Engine
{
    public class Parser
    {
        public List<Token> Tokenize(string input)
        {
            var tokens = new List<Token>();
            int position = 0;

            while (position < input.Length)
            {
                char current = input[position];

                if (char.IsWhiteSpace(current))
                {
                    position++;
                    continue;
                }

                if (char.IsDigit(current) || current == '.')
                {
                    var numberStr = ExtractNumber(input, ref position);
                    tokens.Add(new Token(TokenType.Number, numberStr, position));
                }
                else if (current == '-')
                {
                    bool isUnary = (position == 0) // 맨 앞
                                   || (tokens.Count > 0 && (
                                          tokens[^1].Type == TokenType.LeftParen
                                          || tokens[^1].Type == TokenType.Operator)); // 연산자 뒤

                    if (isUnary)
                    {
                        position++;
                        if (position < input.Length && input[position] == '(')
                        {
                            // -(...) → -1 * (...)
                            tokens.Add(new Token(TokenType.Number, "-1", position));
                            tokens.Add(new Token(TokenType.Operator, "*", position));
                        }
                        else
                        {
                            // -숫자 → 음수 숫자
                            var numberStr = "-" + ExtractNumber(input, ref position);
                            tokens.Add(new Token(TokenType.Number, numberStr, position));
                        }
                    }
                    else
                    {
                        // 이항 연산자
                        tokens.Add(new Token(TokenType.Operator, "-", position));
                        position++;
                    }
                }
                else if (IsOperator(current)) // 이제 여기선 -, +, *, / 중 -는 빠짐
                {
                    tokens.Add(new Token(TokenType.Operator, current.ToString(), position));
                    position++;
                }
                else if (current == '(')
                {
                    tokens.Add(new Token(TokenType.LeftParen, "(", position));
                    position++;
                }
                else if (current == ')')
                {
                    tokens.Add(new Token(TokenType.RightParen, ")", position));
                    position++;
                }
                else if (char.IsLetter(current))
                {
                    var function = ExtractFunction(input, ref position);
                    tokens.Add(new Token(TokenType.Function, function, position));
                }
                else
                {
                    position++;
                }
            }

            return tokens;
        }

        public List<Token> Parse(List<Token> tokens)
        {
            return ApplyShuntingYard(tokens);
        }

        // Shunting Yard 알고리즘 사용, 중위 표기법을 후위 표기법으로 변환
        private List<Token> ApplyShuntingYard(List<Token> tokens)
        {
            var output = new List<Token>();
            var operatorStack = new Stack<Token>();

            foreach (var token in tokens)
            {
                switch (token.Type)
                {
                    case TokenType.Number:
                        output.Add(token);
                        break;

                    case TokenType.Function:
                        operatorStack.Push(token);
                        break;

                    case TokenType.Operator:
                        while (operatorStack.Count > 0 &&
                               operatorStack.Peek().Type != TokenType.LeftParen &&
                               (operatorStack.Peek().Precedence > token.Precedence ||
                                (operatorStack.Peek().Precedence == token.Precedence && token.Associativity == Associativity.Left)))
                        {
                            output.Add(operatorStack.Pop());
                        }
                        operatorStack.Push(token);
                        break;

                    case TokenType.LeftParen:
                        operatorStack.Push(token);
                        break;

                    case TokenType.RightParen:
                        while (operatorStack.Count > 0 && operatorStack.Peek().Type != TokenType.LeftParen)
                        {
                            output.Add(operatorStack.Pop());
                        }
                        if (operatorStack.Count > 0 && operatorStack.Peek().Type == TokenType.LeftParen)
                        {
                            operatorStack.Pop(); 
                        }
                        if (operatorStack.Count > 0 && operatorStack.Peek().Type == TokenType.Function)
                        {
                            output.Add(operatorStack.Pop());
                        }
                        break;
                }
            }

            while (operatorStack.Count > 0)
            {
                output.Add(operatorStack.Pop());
            }

            return output;
        }

        private string ExtractNumber(string input, ref int position)
        {
            int start = position;
            while (position < input.Length && (char.IsDigit(input[position]) || input[position] == '.'))
            {
                position++;
            }
            return input.Substring(start, position - start);
        }

        private string ExtractFunction(string input, ref int position)
        {
            int start = position;
            while (position < input.Length && char.IsLetter(input[position]))
            {
                position++;
            }
            return input.Substring(start, position - start);
        }

        private bool IsOperator(char c)
        {
            return c == '+' || c == '-' || c == '*' || c == '/' || c == '^';
        }
    }
}