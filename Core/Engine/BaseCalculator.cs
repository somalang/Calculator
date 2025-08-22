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
        public abstract CalcValue Evaluate(string expression);
        public abstract bool ValidateExpression(string expression);
    }
}