using Calculator.Core.Engine;
using Calculator.Core.Models;
using Calculator.Core.Services;
using Calculator.UI.Views;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace Calculator.UI.ViewModels
{
    public class CalculatorViewModel : INotifyPropertyChanged
    {
        // 필드
        private string display = string.Empty;
        private string currentInput = string.Empty;
        private readonly BaseCalculator calculator;
        private bool isResultDisplayed;
        private HistoryWindow? historyWindow;
        private readonly IHistoryProvider historyProvider;

        // IHistoryProvider 구현
        public ObservableCollection<HistoryItem> HistoryItems => historyProvider.Items;

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

        public void RemoveHistoryItem(HistoryItem item)
        {
            historyProvider.Remove(item);
        }

        public void ClearHistory()
        {
            historyProvider.Clear();
        }


        // HistoryService의 Items를 직접 노출
        // 명령들
        public ICommand? OpenMenuCommand { get; private set; }

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
        public ICommand? ClearHistoryCommand { get; private set; }
        public ICommand? CopyCommand { get; private set; }
        public ICommand? PasteCommand { get; private set; }
        public ICommand? KeyPressCommand { get; private set; }
        public ICommand? ShowHistoryCommand { get; private set; }
        public ICommand? CloseCommand { get; }

        // 생성자 - 하나로 통합
        public CalculatorViewModel()
        {
            calculator = new StandardCalculator();

            // App.xaml 리소스 대신 App 정적 프로퍼티 사용
            historyProvider = App.HistoryService ?? new HistoryService();

            Display = "0";
            CurrentInput = string.Empty;
            isResultDisplayed = false;

            InitializeCommands();
            OpenMenuCommand = new RelayCommand(OpenMenu);

            CloseCommand = new RelayCommand(CloseWindow);
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
            ClearHistoryCommand = new RelayCommand(ExecuteClearHistory);
            CopyCommand = new RelayCommand(ExecuteCopy);
            PasteCommand = new RelayCommand(ExecutePaste);
            KeyPressCommand = new RelayCommand(ExecuteKeyPress);
            ShowHistoryCommand = new RelayCommand(ExecuteShowHistory);
        }

        // 명령 구현들
        private void CloseWindow(object? parameter)
        {
            // 현재 열려 있는 창을 닫음
            Application.Current.Windows
                .OfType<Window>()
                .FirstOrDefault(w => w.DataContext == this)
                ?.Close();
        }
        private void OpenMenu(object? parameter)
        {
            var currentWindow = Application.Current.Windows
                .OfType<Window>()
                .FirstOrDefault(w => w.DataContext == this);

            if (currentWindow != null)
            {
                var menuWindow = new MenuWindow();
                menuWindow.Show();
                currentWindow.Close();
            }
        }

        private void ExecuteNumber(object? parameter)
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

        private void ExecuteOperator(object? parameter)
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

        private void ExecuteEquals(object? parameter)
        {
            if (string.IsNullOrEmpty(CurrentInput)) return;

            try
            {
                string expression = CurrentInput;
                CalcValue result = calculator.Evaluate(expression);
                HandleCalculationResult(result, expression);
            }
            catch (InvalidOperationException ex)
{
    if (ex.InnerException != null)
    {
        if (ex.InnerException is DivideByZeroException)
        {
            Display = "0으로 나눌 수 없습니다";
        }
        else if (ex.InnerException is OverflowException)
        {
            Display = "오버플로우 발생";
        }
        else
        {
            Display = "계산 오류";
        }
    }
    else
    {
        // InnerException 없으면 메시지에 의존
        if (ex.Message.Contains("0으로 나눌 수 없습니다"))
            Display = "0으로 나눌 수 없습니다";
        else if (ex.Message.Contains("오버플로우"))
            Display = "오버플로우 발생";
        else
            Display = "계산 오류";
    }

    CurrentInput = string.Empty;
    isResultDisplayed = true;
}


        }


        private void ExecuteClear(object? parameter)
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

        private void ExecuteClearAll(object? parameter)
        {
            Display = "0";
            CurrentInput = string.Empty;
            isResultDisplayed = false;
        }

        private void ExecuteBackspace(object? parameter)
        {
            if (isResultDisplayed || string.IsNullOrEmpty(CurrentInput)) return;

            CurrentInput = CurrentInput.Length > 1 ? CurrentInput[..^1] : string.Empty;
            Display = string.IsNullOrEmpty(CurrentInput) ? "0" : CurrentInput;
        }

        private void ExecuteSqrt(object? parameter)
        {
            if (string.IsNullOrEmpty(CurrentInput)) return;

            CurrentInput = $"sqrt({CurrentInput})";
            Display = CurrentInput;
        }

        private void ExecuteSquare(object? parameter)
        {
            if (string.IsNullOrEmpty(CurrentInput)) return;

            CurrentInput = $"sqr({CurrentInput})";
            Display = CurrentInput;
        }

        private void ExecuteReciprocal(object? parameter)
        {
            if (string.IsNullOrEmpty(CurrentInput)) return;

            try
            {
                string expression = CurrentInput;
                CalcValue value = calculator.Evaluate(expression);
                CalcValue result = calculator.Reciprocal(value);
                Display = result.ToString();
                CurrentInput = Display;
                isResultDisplayed = true;

                // 히스토리에 추가
                historyProvider.Add($"1/({expression})", result);
            }
            catch (Exception ex)
            {
                Display = $"오류: {ex.Message}";
                CurrentInput = string.Empty;
                isResultDisplayed = true;
            }
        }

        private void ExecutePercent(object? parameter)
        {
            if (string.IsNullOrEmpty(CurrentInput)) return;

            try
            {
                string expression = CurrentInput;
                CalcValue value = calculator.Evaluate(expression);
                CalcValue result = calculator.Percent(value);
                Display = result.ToString();
                CurrentInput = Display;
                isResultDisplayed = true;

                // 히스토리에 추가
                historyProvider.Add($"{expression}%", result);
            }
            catch (Exception ex)
            {
                Display = $"오류: {ex.Message}";
                CurrentInput = string.Empty;
                isResultDisplayed = true;
            }
        }

        private void ExecuteLeftParen(object? parameter)
        {
            if (isResultDisplayed)
            {
                CurrentInput = string.Empty;
                isResultDisplayed = false;
            }

            CurrentInput += "(";
            Display = CurrentInput;
        }

        private void ExecuteRightParen(object? parameter)
        {
            if (isResultDisplayed || string.IsNullOrEmpty(CurrentInput)) return;

            CurrentInput += ")";
            Display = CurrentInput;
        }

        private void ExecuteToggleSign(object? parameter)
        {
            if (string.IsNullOrEmpty(CurrentInput)) return;

            var tokens = CurrentInput.Trim().Split(' ');
            string lastToken = tokens[^1];

            if (decimal.TryParse(lastToken, out _))
            {
                if (lastToken.StartsWith("-", StringComparison.Ordinal))
                    tokens[^1] = lastToken.Substring(1);
                else
                    tokens[^1] = "-" + lastToken;

                CurrentInput = string.Join(" ", tokens);
                Display = CurrentInput;
            }
        }

        private void ExecuteClearHistory(object? parameter)
        {
            historyProvider.Clear();
        }

        private void ExecuteCopy(object? parameter)
        {
            try
            {
                Clipboard.SetText(Display);
            }
            catch
            {
                Display = "클립보드 복사 오류";
            }
        }

        private void ExecutePaste(object? parameter)
        {
            try
            {
                string clipboardText = Clipboard.GetText();

                if (!string.IsNullOrWhiteSpace(clipboardText))
                {
                    // ✅ 숫자, 연산자, 괄호, 소수점, 공백만 남기고 나머지는 제거
                    string filtered = System.Text.RegularExpressions.Regex.Replace(
                        clipboardText, @"[^0-9+\-*/().\s]", string.Empty);

                    // 공백 제거 후 반영
                    filtered = filtered.Trim();

                    if (!string.IsNullOrEmpty(filtered))
                    {
                        CurrentInput = filtered;
                        Display = CurrentInput;
                        isResultDisplayed = false;
                    }
                    else
                    {
                        Display = "붙여넣기할 유효한 문자가 없음";
                    }
                }
            }
            catch
            {
                Display = "클립보드 붙여넣기 오류";
            }
        }


        private void ExecuteKeyPress(object? parameter)
        {
            if (parameter is string keyString && Enum.TryParse<Key>(keyString, out Key key))
            {
                HandleKeyPress(key);
            }
            else if (parameter is KeyEventArgs keyArgs)
            {
                HandleKeyPress(keyArgs.Key);
            }
            else if (parameter is Key directKey)
            {
                HandleKeyPress(directKey);
            }
        }
        private void HandleCalculationResult(CalcValue result, string expression = "")
        {
            double value = (double)result.Value; // decimal → double 변환

            if (double.IsNaN(value))
            {
                Display = "결과가 유효하지 않습니다 (NaN)";
                CurrentInput = string.Empty;
                isResultDisplayed = true;
                return;
            }
            if (double.IsInfinity(value))
            {
                Display = "∞ (무한대)";
                CurrentInput = string.Empty;
                isResultDisplayed = true;
                return;
            }

            Display = result.ToString();
            CurrentInput = Display;
            isResultDisplayed = true;

            if (!string.IsNullOrEmpty(expression))
                historyProvider.Add(expression, result);
        }


        private void HandleKeyPress(Key key)
        {
            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                switch (key)
                {
                    case Key.C:
                        ExecuteCopy(null);
                        return;
                    case Key.V:
                        ExecutePaste(null);
                        return;
                }
            }

            switch (key)
            {
                case Key.D0:
                case Key.NumPad0:
                    ExecuteNumber("0");
                    break;
                case Key.D1:
                case Key.NumPad1:
                    ExecuteNumber("1");
                    break;
                case Key.D2:
                case Key.NumPad2:
                    ExecuteNumber("2");
                    break;
                case Key.D3:
                case Key.NumPad3:
                    ExecuteNumber("3");
                    break;
                case Key.D4:
                case Key.NumPad4:
                    ExecuteNumber("4");
                    break;
                case Key.D5:
                case Key.NumPad5:
                    ExecuteNumber("5");
                    break;
                case Key.D6:
                case Key.NumPad6:
                    ExecuteNumber("6");
                    break;
                case Key.D7:
                case Key.NumPad7:
                    ExecuteNumber("7");
                    break;
                case Key.D8:
                case Key.NumPad8:
                    ExecuteNumber("8");
                    break;
                case Key.D9:
                case Key.NumPad9:
                    ExecuteNumber("9");
                    break;
                case Key.OemPeriod:
                case Key.Decimal:
                    ExecuteNumber(".");
                    break;
                case Key.OemPlus:
                case Key.Add:
                    ExecuteOperator("+");
                    break;
                case Key.OemMinus:
                case Key.Subtract:
                    ExecuteOperator("-");
                    break;
                case Key.Multiply:
                    ExecuteOperator("*");
                    break;
                case Key.Divide:
                    ExecuteOperator("/");
                    break;
                case Key.Enter:
                    ExecuteEquals(null);
                    break;
                case Key.Escape:
                    ExecuteClearAll(null);
                    break;
                case Key.Back:
                    ExecuteBackspace(null);
                    break;
                case Key.Delete:
                    ExecuteClear(null);
                    break;
            }
        }

        private void ExecuteShowHistory(object? parameter)
        {
            if (historyWindow != null)
            {
                historyWindow.Activate();
                historyWindow.Focus();
                return;
            }

            // HistoryViewModel에 선택 시 입력을 업데이트하는 Action 전달
            var historyViewModel = new HistoryViewModel(
                historyProvider,
                expression =>
                {
                    CurrentInput = expression;
                    Display = expression;
                    isResultDisplayed = false;
                });

            historyWindow = new HistoryWindow();
            historyWindow.DataContext = historyViewModel;
            historyWindow.Owner = Application.Current.MainWindow;
            historyWindow.Closed += (s, e) => historyWindow = null;
            historyWindow.Show();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
