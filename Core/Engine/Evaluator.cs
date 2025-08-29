// 토큰 리스트를 실제로 계산하여 결과를 반환 - 핵심 계산 엔진

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
                "^" => new CalcValue((decimal)Math.Pow((double)left.Value, (double)right.Value)), // 추가
                _ => throw new NotSupportedException($"지원하지 않는 연산자: {operatorSymbol}")
            };
        }

        private CalcValue ApplyFunction(string function, CalcValue operand)
        {
            double val = (double)operand.Value; // decimal → double 변환

            return function.ToLower() switch
            {
                "sqrt" => new CalcValue((decimal)Math.Sqrt(val)),
                "sqr" => operand * operand,
                "round" => new CalcValue(Math.Round(operand.Value)),
                "floor" => new CalcValue(Math.Floor(operand.Value)),
                "ceil" => new CalcValue(Math.Ceiling(operand.Value)),
                "abs" => new CalcValue(Math.Abs(operand.Value)),
                "sign" => new CalcValue(Math.Sign(operand.Value)),

                // 삼각함수 (라디안 기준)
                "sin" => new CalcValue((decimal)Math.Sin(val)),
                "cos" => new CalcValue((decimal)Math.Cos(val)),
                "tan" => new CalcValue((decimal)Math.Tan(val)),

                // 역삼각함수 (입력 -1 ~ 1)
                "asin" => SafeTrig(Math.Asin, val),
                "acos" => SafeTrig(Math.Acos, val),
                "atan" => new CalcValue((decimal)Math.Atan(val)),

                // 로그/지수
                "log" => SafeLog10(val),
                "ln" => SafeLog(val),
                "exp" => new CalcValue((decimal)Math.Exp(val)),

                _ => throw new NotSupportedException($"지원하지 않는 함수: {function}")
            };
        }

        // 로그 입력값 안전 처리
        private CalcValue SafeLog10(double val)
        {
            if (val <= 0) throw new ArgumentException("log는 0보다 큰 값만 허용");
            return new CalcValue((decimal)Math.Log10(val));
        }

        private CalcValue SafeLog(double val)
        {
            if (val <= 0) throw new ArgumentException("ln은 0보다 큰 값만 허용");
            return new CalcValue((decimal)Math.Log(val));
        }

        // asin, acos 범위 안전 처리
        private CalcValue SafeTrig(Func<double, double> func, double val)
        {
            if (val < -1.0 || val > 1.0)
                throw new ArgumentException("asin/acos 입력값은 -1과 1 사이여야 함");
            return new CalcValue((decimal)func(val));
        }

    }
}
