using Calculator.Core.Models;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Calculator.Core.Engine
{
    public class Evaluator
    {
        public CalcValue Evaluate(List<Token> rpnTokens)
        {
            var stack = new Stack<CalcValue>();

            foreach (var token in rpnTokens)
            {
                switch (token.Type)
                {
                    case TokenType.Number:
                        if (decimal.TryParse(token.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal number))
                        {
                            stack.Push(new CalcValue(number));
                        }
                        break;

                    case TokenType.Operator:
                        if (stack.Count >= 2)
                        {
                            var right = stack.Pop();
                            var left = stack.Pop();
                            var result = ApplyOperator(token.Value, left, right);
                            stack.Push(result);
                        }
                        break;

                    case TokenType.Function:
                        if (stack.Count >= 1)
                        {
                            var operand = stack.Pop();
                            var result = ApplyFunction(token.Value, operand);
                            stack.Push(result);
                        }
                        break;
                }
            }

            return stack.Count > 0 ? stack.Pop() : CalcValue.ZERO;
        }

        private CalcValue ApplyOperator(string operatorSymbol, CalcValue left, CalcValue right)
        {
            return operatorSymbol switch
            {
                "+" => left + right,
                "-" => left - right,
                "*" => left * right,
                "/" => left / right,
                _ => throw new NotSupportedException($"지원하지 않는 연산자: {operatorSymbol}")
            };
        }

        private CalcValue ApplyFunction(string function, CalcValue operand)
        {
            return function switch
            {
                "sqrt" => operand.Sqrt(),
                "sqr" => operand.Square(),
                _ => throw new NotSupportedException($"지원하지 않는 함수: {function}")
            };
        }
    }
}