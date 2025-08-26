using Calculator.Core.Engine;
using Calculator.Core.Models;
using Calculator.UI.Views;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace Calculator.UI.ViewModels
{
    public class AdvancedCalculatorViewModel : INotifyPropertyChanged
    {
        private string display = string.Empty;
        private string currentInput = string.Empty;

        private readonly AdvancedCalculator calculator;
        private bool isResultDisplayed;
        private bool isRadianMode = true;

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

        public bool IsRadianMode
        {
            get => isRadianMode;
            set
            {
                isRadianMode = value;
                OnPropertyChanged();
            }
        }
        public ICommand? OpenMenuCommand { get; }

        // Commands
        public ICommand? NumberCommand { get; private set; }
        public ICommand? OperatorCommand { get; private set; }
        public ICommand? EqualsCommand { get; private set; }
        public ICommand? ClearCommand { get; private set; }
        public ICommand? ClearAllCommand { get; private set; }
        public ICommand? BackspaceCommand { get; private set; }
        public ICommand? SqrtCommand { get; private set; }
        public ICommand? SquareCommand { get; private set; }
        public ICommand? ReciprocalCommand { get; private set; }
        public ICommand? PercentCommand { get; private set; }
        public ICommand? LeftParenCommand { get; private set; }
        public ICommand? RightParenCommand { get; private set; }
        public ICommand? ToggleSignCommand { get; private set; }

        // Rounding functions
        public ICommand? RoundCommand { get; private set; }
        public ICommand? FloorCommand { get; private set; }
        public ICommand? CeilCommand { get; private set; }
        public ICommand? AbsCommand { get; private set; }
        public ICommand? SignCommand { get; private set; }

        // Trigonometric functions
        public ICommand? SinCommand { get; private set; }
        public ICommand? CosCommand { get; private set; }
        public ICommand? TanCommand { get; private set; }
        public ICommand? AsinCommand { get; private set; }
        public ICommand? AcosCommand { get; private set; }
        public ICommand? AtanCommand { get; private set; }

        // Logarithmic functions
        public ICommand? LogCommand { get; private set; }
        public ICommand? LnCommand { get; private set; }
        public ICommand? ExpCommand { get; private set; }
        public ICommand? PowCommand { get; private set; }

        // Constants
        public ICommand? PiCommand { get; private set; }
        public ICommand? ECommand { get; private set; }

        // Mode toggle
        public ICommand? ToggleAngleModeCommand { get; private set; }

        public AdvancedCalculatorViewModel()
        {
            calculator = new AdvancedCalculator();
            Display = "0";
            CurrentInput = string.Empty;
            isResultDisplayed = false;

            InitializeCommands();
            OpenMenuCommand = new RelayCommand(OpenMenu);

        }

        private void InitializeCommands()
        {
            NumberCommand = new RelayCommand(ExecuteNumber);
            OperatorCommand = new RelayCommand(ExecuteOperator);
            EqualsCommand = new RelayCommand(ExecuteEquals);
            ClearCommand = new RelayCommand(ExecuteClear);
            ClearAllCommand = new RelayCommand(ExecuteClearAll);
            BackspaceCommand = new RelayCommand(ExecuteBackspace);
            SqrtCommand = new RelayCommand(ExecuteSqrt);
            SquareCommand = new RelayCommand(ExecuteSquare);
            ReciprocalCommand = new RelayCommand(ExecuteReciprocal);
            PercentCommand = new RelayCommand(ExecutePercent);
            LeftParenCommand = new RelayCommand(ExecuteLeftParen);
            RightParenCommand = new RelayCommand(ExecuteRightParen);
            ToggleSignCommand = new RelayCommand(ExecuteToggleSign);

            // Rounding
            RoundCommand = new RelayCommand(ExecuteRound);
            FloorCommand = new RelayCommand(ExecuteFloor);
            CeilCommand = new RelayCommand(ExecuteCeil);
            AbsCommand = new RelayCommand(ExecuteAbs);
            SignCommand = new RelayCommand(ExecuteSign);

            // Trigonometric
            SinCommand = new RelayCommand(ExecuteSin);
            CosCommand = new RelayCommand(ExecuteCos);
            TanCommand = new RelayCommand(ExecuteTan);
            AsinCommand = new RelayCommand(ExecuteAsin);
            AcosCommand = new RelayCommand(ExecuteAcos);
            AtanCommand = new RelayCommand(ExecuteAtan);

            // Logarithmic
            LogCommand = new RelayCommand(ExecuteLog);
            LnCommand = new RelayCommand(ExecuteLn);
            ExpCommand = new RelayCommand(ExecuteExp);
            PowCommand = new RelayCommand(ExecutePow);

            // Constants
            PiCommand = new RelayCommand(ExecutePi);
            ECommand = new RelayCommand(ExecuteE);

            // Mode
            ToggleAngleModeCommand = new RelayCommand(ExecuteToggleAngleMode);
        }
        private void OpenMenu(object parameter)
        {
            if (parameter is Window currentWindow)
            {
                var menuWindow = new MenuWindow();
                menuWindow.Show();

                currentWindow.Close();
            }
        }
        private void ExecuteNumber(object parameter)
        {
            string? number = parameter?.ToString();
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
            string? operatorSymbol = parameter?.ToString();
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
                string expression = CurrentInput;
                CalcValue result = calculator.Evaluate(expression);
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
            ExecuteFunction("sqrt");
        }

        private void ExecuteSquare(object parameter)
        {
            ExecuteFunction("sqr");
        }

        private void ExecuteReciprocal(object parameter)
        {
            ExecuteUnaryOperation(calculator.Reciprocal, "1/");
        }

        private void ExecutePercent(object parameter)
        {
            ExecuteUnaryOperation(calculator.Percent, "%");
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
                if (lastToken.StartsWith("-"))
                    tokens[^1] = lastToken.Substring(1);
                else
                    tokens[^1] = "-" + lastToken;

                CurrentInput = string.Join(" ", tokens);
                Display = CurrentInput;
            }
        }

        // Rounding functions
        private void ExecuteRound(object parameter)
        {
            ExecuteFunction("round");
        }

        private void ExecuteFloor(object parameter)
        {
            ExecuteFunction("floor");
        }

        private void ExecuteCeil(object parameter)
        {
            ExecuteFunction("ceil");
        }

        private void ExecuteAbs(object parameter)
        {
            ExecuteFunction("abs");
        }

        private void ExecuteSign(object parameter)
        {
            ExecuteFunction("sign");
        }

        // Trigonometric functions
        private void ExecuteSin(object parameter)
        {
            ExecuteFunction("sin");
        }

        private void ExecuteCos(object parameter)
        {
            ExecuteFunction("cos");
        }

        private void ExecuteTan(object parameter)
        {
            ExecuteFunction("tan");
        }

        private void ExecuteAsin(object parameter)
        {
            ExecuteFunction("asin");
        }

        private void ExecuteAcos(object parameter)
        {
            ExecuteFunction("acos");
        }

        private void ExecuteAtan(object parameter)
        {
            ExecuteFunction("atan");
        }

        // Logarithmic functions
        private void ExecuteLog(object parameter)
        {
            ExecuteFunction("log");
        }

        private void ExecuteLn(object parameter)
        {
            ExecuteFunction("ln");
        }

        private void ExecuteExp(object parameter)
        {
            ExecuteFunction("exp");
        }

        private void ExecutePow(object parameter)
        {
            if (isResultDisplayed)
            {
                isResultDisplayed = false;
            }

            if (!string.IsNullOrEmpty(CurrentInput))
            {
                CurrentInput += " ^ ";
                Display = CurrentInput;
            }
        }

        // Constants
        private void ExecutePi(object parameter)
        {
            if (isResultDisplayed)
            {
                CurrentInput = string.Empty;
                isResultDisplayed = false;
            }

            CurrentInput += Math.PI.ToString();
            Display = CurrentInput;
        }

        private void ExecuteE(object parameter)
        {
            if (isResultDisplayed)
            {
                CurrentInput = string.Empty;
                isResultDisplayed = false;
            }

            CurrentInput += Math.E.ToString();
            Display = CurrentInput;
        }

        // Mode toggle
        private void ExecuteToggleAngleMode(object parameter)
        {
            IsRadianMode = !IsRadianMode;
        }

        // Helper methods
        private void ExecuteFunction(string functionName)
        {
            if (string.IsNullOrEmpty(CurrentInput)) return;

            CurrentInput = $"{functionName}({CurrentInput})";
            Display = CurrentInput;
        }

        private void ExecuteUnaryOperation(Func<CalcValue, CalcValue> operation, string operatorSymbol)
        {
            if (string.IsNullOrEmpty(CurrentInput)) return;

            try
            {
                string expression = CurrentInput;
                CalcValue value = calculator.Evaluate(expression);
                CalcValue result = operation(value);
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

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}