using Calculator.Core.Models;
using System;

namespace Calculator.Core.Engine
{
    public class AdvancedCalculator : BaseCalculator
    {
        private bool isRadianMode = true;

        public bool IsRadianMode
        {
            get => isRadianMode;
            set => isRadianMode = value;
        }

        public override CalcValue Evaluate(string expression)
        {
            try
            {
                if (!ValidateExpression(expression))
                    throw new ArgumentException("잘못된 수식 입력");

                // 고급 함수들을 처리하기 위해 전처리
                expression = PreprocessAdvancedFunctions(expression);

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

        private string PreprocessAdvancedFunctions(string expression)
        {
            // 고급 함수들을 기본 연산으로 변환하거나 특별 처리
            // 이 부분은 실제로는 Parser와 Evaluator에서 처리되어야 하지만
            // 여기서는 간단한 전처리만 수행
            return expression;
        }

        // 라운딩 함수들
        public CalcValue Round(CalcValue value, int digits = 0)
        {
            return new CalcValue(Math.Round(value.Value, digits));
        }

        public CalcValue Floor(CalcValue value)
        {
            return new CalcValue(Math.Floor(value.Value));
        }

        public CalcValue Ceiling(CalcValue value)
        {
            return new CalcValue(Math.Ceiling(value.Value));
        }

        public CalcValue Abs(CalcValue value)
        {
            return new CalcValue(Math.Abs(value.Value));
        }

        public CalcValue Sign(CalcValue value)
        {
            return new CalcValue(Math.Sign(value.Value));
        }

        // 삼각함수
        public CalcValue Sin(CalcValue value)
        {
            double angle = (double)value.Value;
            if (!isRadianMode)
                angle = DegreesToRadians(angle);

            return new CalcValue((decimal)Math.Sin(angle));
        }

        public CalcValue Cos(CalcValue value)
        {
            double angle = (double)value.Value;
            if (!isRadianMode)
                angle = DegreesToRadians(angle);

            return new CalcValue((decimal)Math.Cos(angle));
        }

        public CalcValue Tan(CalcValue value)
        {
            double angle = (double)value.Value;
            if (!isRadianMode)
                angle = DegreesToRadians(angle);

            return new CalcValue((decimal)Math.Tan(angle));
        }

        public CalcValue Asin(CalcValue value)
        {
            double result = Math.Asin((double)value.Value);
            if (!isRadianMode)
                result = RadiansToDegrees(result);

            return new CalcValue((decimal)result);
        }

        public CalcValue Acos(CalcValue value)
        {
            double result = Math.Acos((double)value.Value);
            if (!isRadianMode)
                result = RadiansToDegrees(result);

            return new CalcValue((decimal)result);
        }

        public CalcValue Atan(CalcValue value)
        {
            double result = Math.Atan((double)value.Value);
            if (!isRadianMode)
                result = RadiansToDegrees(result);

            return new CalcValue((decimal)result);
        }

        // 로그 함수
        public CalcValue Log(CalcValue value)
        {
            if (value.Value <= 0)
                throw new InvalidOperationException("로그 함수의 인수는 양수여야 함");

            return new CalcValue((decimal)Math.Log10((double)value.Value));
        }

        public CalcValue Ln(CalcValue value)
        {
            if (value.Value <= 0)
                throw new InvalidOperationException("자연로그 함수의 인수는 양수여야 함");

            return new CalcValue((decimal)Math.Log((double)value.Value));
        }

        public CalcValue Exp(CalcValue value)
        {
            return new CalcValue((decimal)Math.Exp((double)value.Value));
        }

        public CalcValue Power(CalcValue baseValue, CalcValue exponent)
        {
            return new CalcValue((decimal)Math.Pow((double)baseValue.Value, (double)exponent.Value));
        }

        // 각도 변환 헬퍼 메서드
        private double DegreesToRadians(double degrees)
        {
            return degrees * Math.PI / 180.0;
        }

        private double RadiansToDegrees(double radians)
        {
            return radians * 180.0 / Math.PI;
        }

        // 상수
        public CalcValue Pi => new CalcValue((decimal)Math.PI);
        public CalcValue E => new CalcValue((decimal)Math.E);
    }
}