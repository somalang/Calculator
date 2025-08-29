// 계산 기능의 기본 골격

using System;
using Calculator.Core.Models;

namespace Calculator.Core.Engine
{
    public abstract class BaseCalculator
    {
        protected readonly Parser parser;
        protected readonly Evaluator evaluator;
        protected readonly CalculationHistory history;

        protected BaseCalculator()
        {
            parser = new Parser();
            evaluator = new Evaluator();
            history = new CalculationHistory();
        }

        // 히스토리 접근 속성
        public CalculationHistory History => history;

        // 기본 연산들 (CalcValue의 연산자 사용)
        public virtual CalcValue Add(CalcValue left, CalcValue right)
        {
            return left + right;
        }

        public virtual CalcValue Subtract(CalcValue left, CalcValue right)
        {
            return left - right;
        }

        public virtual CalcValue Multiply(CalcValue left, CalcValue right)
        {
            return left * right;
        }

        public virtual CalcValue Divide(CalcValue left, CalcValue right)
        {
            return left / right;
        }

        public virtual CalcValue Sqrt(CalcValue value)
        {
            return value.Sqrt();
        }

        public virtual CalcValue Square(CalcValue value)
        {
            return value.Square();
        }

        public virtual CalcValue Reciprocal(CalcValue value)
        {
            return value.Reciprocal();
        }

        public virtual CalcValue Percent(CalcValue value)
        {
            return value.Percent();
        }

        // 추상 메서드
        public abstract CalcValue Evaluate(string expression);
        public abstract bool ValidateExpression(string expression);
    }
}