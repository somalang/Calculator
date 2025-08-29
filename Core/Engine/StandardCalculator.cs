//실제 수식 평가 및 유효성 검사 

using Calculator.Core.Models;
using System;

namespace Calculator.Core.Engine
{
    public class StandardCalculator : BaseCalculator
    {
        public override CalcValue Evaluate(string expression)
        {
            try
            {
                if (!ValidateExpression(expression))
                    throw new ArgumentException("잘못된 수식 입력");

                var tokens = parser.Tokenize(expression);
                var rpnTokens = parser.Parse(tokens);
                var result = evaluator.Evaluate(rpnTokens);

                // 히스토리에 추가
                history.AddItem(expression, result);

                return result;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"계산 오류: {ex.Message}");
            }
        }

        public override bool ValidateExpression(string expression)
        {
            if (string.IsNullOrWhiteSpace(expression))
                return false;

            // 괄호 균형 검사
            int parenthesesCount = 0;
            foreach (char c in expression)
            {
                if (c == '(') parenthesesCount++;
                else if (c == ')') parenthesesCount--;

                if (parenthesesCount < 0) return false;
            }

            return parenthesesCount == 0;
        }
    }
}