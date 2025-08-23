using System;
using Calculator.Core.Models;

namespace Calculator.Core.Engine
{
    public abstract class BaseCalculator
    {
        protected readonly Parser parser;
        protected readonly Evaluator evaluator;

        protected BaseCalculator()
        {
            parser = new Parser();
            evaluator = new Evaluator();
        }

        // 공통 기본 연산들
        public virtual CalcValue Add(CalcValue left, CalcValue right)
        {
            if (left == null || right == null) throw new ArgumentNullException();
            return new CalcValue(left.Value + right.Value);
        }

        public virtual CalcValue Subtract(CalcValue left, CalcValue right)
        {
            if (left == null || right == null) throw new ArgumentNullException();
            return new CalcValue(left.Value - right.Value);
        }

        public virtual CalcValue Multiply(CalcValue left, CalcValue right)
        {
            if (left == null || right == null) throw new ArgumentNullException();
            return new CalcValue(left.Value * right.Value);
        }

        public virtual CalcValue Divide(CalcValue left, CalcValue right)
        {
            if (left == null || right == null) throw new ArgumentNullException();
            if (right.Value == 0) throw new DivideByZeroException("0으로 나눌 수 없음");
            return new CalcValue(left.Value / right.Value);
        }

        public virtual CalcValue Sqrt(CalcValue value)
        {
            if (value == null) throw new ArgumentNullException();
            if (value.Value < 0) throw new InvalidOperationException("음수의 제곱근을 구할 수 없음");
            return new CalcValue((decimal)Math.Sqrt((double)value.Value));
        }

        public virtual CalcValue Square(CalcValue value)
        {
            if (value == null) throw new ArgumentNullException();
            return new CalcValue(value.Value * value.Value);
        }

        public virtual CalcValue Percent(CalcValue value)
        {
            if (value == null) throw new ArgumentNullException();
            return new CalcValue(value.Value / 100);
        }

        // 추상 메서드 (표현식 평가)
        public abstract CalcValue Evaluate(string expression);
        public abstract bool ValidateExpression(string expression);
    }
}