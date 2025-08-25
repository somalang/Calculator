using Calculator.Core.Engine;
using Calculator.Core.Models;
using Calculator.Core.Services;
using Calculator.UI.Views;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace Calculator.UI.ViewModels
{
    public class StandardCalculatorViewModel : INotifyPropertyChanged
    {
        private string display;
        private string currentInput;
        private readonly StandardCalculator calculator;
        private readonly HistoryService historyService;
        private bool isResultDisplayed;

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

        public HistoryService HistoryService => historyService;

        // Commands
        public ICommand NumberCommand { get; private set; }
        public ICommand OperatorCommand { get; private set; }
        public ICommand EqualsCommand { get; private set; }
        public ICommand ClearCommand { get; private set; }
        public ICommand ClearAllCommand { get; private set; }
        public ICommand BackspaceCommand { get; private set; }
        public ICommand SqrtCommand { get; private set; }
        public ICommand SquareCommand { get; private set; }
        public ICommand ReciprocalCommand { get; private set; }
        public ICommand PercentCommand { get; private set; }
        public ICommand ToggleSignCommand { get; private set; }
        public ICommand DecimalCommand { get; private set; }
        public ICommand MenuCommand { get; private set; }
        public ICommand HistoryCommand { get; private set; }

        public StandardCalculatorViewModel()
        {
            calculator = new StandardCalculator();
            historyService = new HistoryService();
            Display = "0";
            CurrentInput = string.Empty;
            isResultDisplayed = false;

            InitializeCommands();
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
            ToggleSignCommand = new RelayCommand(ExecuteToggleSign);
            DecimalCommand = new RelayCommand(ExecuteDecimal);
            MenuCommand = new RelayCommand(ExecuteMenu);
            HistoryCommand = new RelayCommand(ExecuteHistory);
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

            if (CurrentInput == "0" && number != "0")
                CurrentInput = string.Empty;

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
                string expression = CurrentInput;
                CalcValue result = calculator.Evaluate(expression);

                // 히스토리에 추가
                historyService.Add(expression, result);

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
            ExecuteUnaryOperation(calculator.Sqrt, "√");
        }

        private void ExecuteSquare(object parameter)
        {
            ExecuteUnaryOperation(calculator.Square, "sqr");
        }

        private void ExecuteReciprocal(object parameter)
        {
            ExecuteUnaryOperation(calculator.Reciprocal, "1/");
        }

        private void ExecutePercent(object parameter)
        {
            ExecuteUnaryOperation(calculator.Percent, "%");
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

        private void ExecuteDecimal(object parameter)
        {
            if (isResultDisplayed)
            {
                CurrentInput = "0";
                isResultDisplayed = false;
            }

            if (string.IsNullOrEmpty(CurrentInput))
                CurrentInput = "0";

            var tokens = CurrentInput.Split(' ');
            string lastToken = tokens[^1];

            if (!lastToken.Contains("."))
            {
                tokens[^1] = lastToken + ".";
                CurrentInput = string.Join(" ", tokens);
                Display = CurrentInput;
            }
        }

        private void ExecuteMenu(object parameter)
        {
            var menuWindow = new MenuWindow();
            menuWindow.Owner = Application.Current.MainWindow;
            menuWindow.ShowDialog();
        }

        private void ExecuteHistory(object parameter)
        {
            // 히스토리 창을 여는 로직 (필요시 구현)
            MessageBox.Show($"최근 계산: {historyService.Items.Count}개", "계산 히스토리");
        }

        private void ExecuteUnaryOperation(Func<CalcValue, CalcValue> operation, string operatorSymbol)
        {
            if (string.IsNullOrEmpty(CurrentInput)) return;

            try
            {
                string expression = CurrentInput;
                CalcValue value = calculator.Evaluate(expression);
                CalcValue result = operation(value);

                // 히스토리에 추가
                historyService.Add($"{operatorSymbol}({expression})", result);

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

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}