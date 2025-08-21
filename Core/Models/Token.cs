namespace Calculator.Core.Models
{
    public enum TokenType
    {
        Number,
        Operator,
        LeftParen,
        RightParen,
        Function
    }

    public enum Associativity
    {
        Left,
        Right
    }

    public class Token
    {
        public TokenType Type { get; set; }
        public string Value { get; set; }
        public int Precedence { get; set; }
        public Associativity Associativity { get; set; }
        public int Position { get; set; }

        public Token(TokenType type, string value, int position = 0)
        {
            Type = type;
            Value = value;
            Position = position;
            SetPrecedenceAndAssociativity();
        }

        private void SetPrecedenceAndAssociativity()
        {
            switch (Value)
            {
                case "+":
                case "-":
                    Precedence = 1;
                    Associativity = Associativity.Left;
                    break;
                case "*":
                case "/":
                    Precedence = 2;
                    Associativity = Associativity.Left;
                    break;
                case "sqrt":
                case "sqr":
                    Precedence = 3;
                    Associativity = Associativity.Right;
                    break;
                default:
                    Precedence = 0;
                    Associativity = Associativity.Left;
                    break;
            }
        }
    }
}