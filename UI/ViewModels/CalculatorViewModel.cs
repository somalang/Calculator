using Calculator.Core.Engine;
using Calculator.Core.Models;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Calculator.UI.ViewModels
{
    public class CalculatorViewModel : INotifyPropertyChanged
    {
        // 필드
        private string display;
        private string currentInput;
        private readonly BaseCalculator calculator;
        private bool isResultDisplayed;
        // 속성
        public string Display
        {
            get => display;
            set
            {
                display = value;
                OnPropertyChanged();
            }
        }
        public string CurrentInput
        {
            get => currentInput;
            set
            {
                currentInput = value;
                OnPropertyChanged();
            }
        }
        // 명령
        public ICommand NumberCommand { get; }
        public ICommand OperatorCommand { get; }
        public ICommand EqualsCommand { get; }
        public ICommand ClearCommand { get; }
        public ICommand ClearAllCommand { get; }
        public ICommand BackspaceCommand { get; }
        public ICommand SqrtCommand { get; }
        public ICommand SquareCommand { get; }
        public ICommand LeftParenCommand { get; }
        public ICommand RightParenCommand { get; }
        public ICommand ToggleSignCommand { get; }


        // 생성자
        public CalculatorViewModel()
        {
            calculator = new StandardCalculator();
            Display = "0";
            CurrentInput = string.Empty;
            isResultDisplayed = false;

            NumberCommand = new RelayCommand(ExecuteNumber);
            OperatorCommand = new RelayCommand(ExecuteOperator);
            EqualsCommand = new RelayCommand(ExecuteEquals);
            ClearCommand = new RelayCommand(ExecuteClear);
            ClearAllCommand = new RelayCommand(ExecuteClearAll);
            BackspaceCommand = new RelayCommand(ExecuteBackspace);
            SqrtCommand = new RelayCommand(ExecuteSqrt);
            SquareCommand = new RelayCommand(ExecuteSquare);
            LeftParenCommand = new RelayCommand(ExecuteLeftParen);
            RightParenCommand = new RelayCommand(ExecuteRightParen);
            ToggleSignCommand = new RelayCommand(ExecuteToggleSign);
        }
        private void ExecuteNumber(object parameter)
        {
            string number = parameter?.ToString();
            if (string.IsNullOrEmpty(number)) return;

            if (isResultDisplayed)
            {
                CurrentInput = string.Empty;
                isResultDisplayed = false;
            }

            if (number == "." && CurrentInput.Contains(".")) return;
            if (CurrentInput == "0" && number != ".") CurrentInput = string.Empty;

            CurrentInput += number;
            Display = CurrentInput;
        }
        private void ExecuteOperator(object parameter)
        {
            string operatorSymbol = parameter?.ToString();
            if (string.IsNullOrEmpty(operatorSymbol)) return;

            if (isResultDisplayed)
            {
                isResultDisplayed = false;
            }

            if (!string.IsNullOrEmpty(CurrentInput))
            {
                CurrentInput += $" {operatorSymbol} ";
                Display = CurrentInput;
            }
        }
        private void ExecuteEquals(object parameter)
        {
            if (string.IsNullOrEmpty(CurrentInput)) return;

            try
            {
                CalcValue result = calculator.Evaluate(CurrentInput);
                Display = result.ToString();
                CurrentInput = Display;
                isResultDisplayed = true;
            }
            catch (Exception ex)
            {
                Display = $"오류: {ex.Message}";
                CurrentInput = string.Empty;
                isResultDisplayed = true;
            }
        }
        private void ExecuteClear(object parameter)
        {
            if (string.IsNullOrEmpty(CurrentInput)) return;

            // 공백 기준으로 쪼개고 마지막 토큰만 제거
            var tokens = CurrentInput.Trim().Split(' ');
            if (tokens.Length > 1)
            {
                CurrentInput = string.Join(" ", tokens, 0, tokens.Length - 1);
            }
            else
            {
                CurrentInput = string.Empty;
            }

            Display = string.IsNullOrEmpty(CurrentInput) ? "0" : CurrentInput;
            isResultDisplayed = false;
        }

        private void ExecuteClearAll(object parameter)
        {
            Display = "0";
            CurrentInput = string.Empty;
            isResultDisplayed = false;
        }

        private void ExecuteBackspace(object parameter)
        {
            if (isResultDisplayed || string.IsNullOrEmpty(CurrentInput)) return;

            CurrentInput = CurrentInput.Length > 1 ? CurrentInput[..^1] : string.Empty;
            Display = string.IsNullOrEmpty(CurrentInput) ? "0" : CurrentInput;
        }
        private void ExecuteSqrt(object parameter)
        {
            if (string.IsNullOrEmpty(CurrentInput)) return;

            CurrentInput = $"sqrt({CurrentInput})";
            Display = CurrentInput;
        }
        private void ExecuteSquare(object parameter)
        {
            if (string.IsNullOrEmpty(CurrentInput)) return;

            CurrentInput = $"sqr({CurrentInput})";
            Display = CurrentInput;
        }
        private void ExecuteLeftParen(object parameter)
        {
            if (isResultDisplayed)
            {
                CurrentInput = string.Empty;
                isResultDisplayed = false;
            }

            CurrentInput += "(";
            Display = CurrentInput;
        }
        private void ExecuteRightParen(object parameter)
        {
            if (isResultDisplayed || string.IsNullOrEmpty(CurrentInput)) return;

            CurrentInput += ")";
            Display = CurrentInput;
        }
        private void ExecuteToggleSign(object parameter)
        {
            if (string.IsNullOrEmpty(CurrentInput)) return;
        
            var tokens = CurrentInput.Trim().Split(' ');
            string lastToken = tokens[^1];
        
            if (decimal.TryParse(lastToken, out _))
            {
                // 음수면 양수로, 양수면 음수로
                if (lastToken.StartsWith("-", StringComparison.Ordinal))
                    tokens[^1] = lastToken.Substring(1);
                else
                    tokens[^1] = "-" + lastToken;
        
                CurrentInput = string.Join(" ", tokens);
                Display = CurrentInput;
            }
        }

        // INotifyPropertyChanged 구현
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
