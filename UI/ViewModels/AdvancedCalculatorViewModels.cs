using Calculator.Core.Engine;
using Calculator.Core.Models;
using Calculator.Core.Services;
using Calculator.UI.Views;
using System;
using System.Collections.ObjectModel;
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
        private HistoryWindow? historyWindow;

        // HistoryService 직접 생성하거나 App에서 가져오기
        private readonly IHistoryProvider historyProvider;

        public ObservableCollection<HistoryItem> HistoryItems => historyProvider.Items;
        public string Display
        {
            get => display;
            set
            {
                if (!string.IsNullOrEmpty(value) && value.Length > 20)
                {
                    display = "너무 길어서 입력을 받을 수 없습니다";
                    CurrentInput = string.Empty; // 입력 초기화
                }
                else
                {
                    display = value;
                }

                UpdateDisplayFontSize();
                OnPropertyChanged();
                OnPropertyChanged(nameof(DisplayTrimmed));
            }
        }


        private void UpdateDisplayFontSize()
        {
            if (string.IsNullOrEmpty(display))
            {
                DisplayFontSize = 32;
                return;
            }

            int length = display.Length;

            if (length <= 7)
            {
                DisplayFontSize = 32; // 기본 크기
            }
            else if (length <= 20)
            {
                // 7~20 글자 구간 → 선형으로 줄이기 (32 → 19)
                double factor = (double)(length - 7) / (20 - 7);
                DisplayFontSize = 32 - (13 * factor); // 32 → 19 (차이 13)
            }
            else
            {
                DisplayFontSize = 19; // 최소 크기
            }
        }

        public string DisplayTrimmed
        {
            get
            {
                if (string.IsNullOrEmpty(display))
                    return "0";

                if (display.Length <= 30)
                    return display;

                return display.Substring(0, 20) + "...";
            }
        }

        private double displayFontSize = 32;
        public double DisplayFontSize
        {
            get => displayFontSize;
            private set
            {
                displayFontSize = value;
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
        public ICommand? OpenMenuCommand { get; private set; }
        public ICommand? CloseCommand { get; }


        // Commands (기존 코드와 동일)
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
        public ICommand? ClearHistoryCommand { get; private set; }
        public ICommand? ShowHistoryCommand { get; private set; }
        public ICommand? KeyPressCommand { get; private set; }
        public ICommand? CopyCommand { get; private set; }

        public ICommand? PasteCommand { get; private set; }

        public AdvancedCalculatorViewModel()
        {
            calculator = new AdvancedCalculator();
            Display = "0";
            CurrentInput = string.Empty;
            isResultDisplayed = false;

            // HistoryService를 App에서 가져오거나 새로 생성
            historyProvider = App.HistoryService ?? new HistoryService();

            InitializeCommands();
            CloseCommand = new RelayCommand(CloseWindow);
            OpenMenuCommand = new RelayCommand(OpenMenu);

        }

        // 생성자 오버로드 - 의존성 주입을 위해
        public AdvancedCalculatorViewModel(IHistoryProvider historyProvider)
        {
            calculator = new AdvancedCalculator();
            Display = "0";
            CurrentInput = string.Empty;
            isResultDisplayed = false;

            this.historyProvider = historyProvider;

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
            KeyPressCommand = new RelayCommand(ExecuteKeyPress);
            CopyCommand = new RelayCommand(ExecuteCopy);

            PasteCommand = new RelayCommand(ExecutePaste);

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
            ShowHistoryCommand = new RelayCommand(ExecuteShowHistory);
            ClearHistoryCommand = new RelayCommand(ExecuteClearHistory);
        }
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
                Display = result.ToString();

                // Add to history - 여기가 중요!
                historyProvider.Add(expression, result);

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

        // 나머지 Execute 메서드들은 기존과 동일...
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

        private void ExecuteClearHistory(object? parameter)
        {
            historyProvider.Clear();
        }

        // 나머지 함수들은 기존과 동일하므로 생략...
        private void ExecuteSqrt(object? parameter) => ExecuteFunction("sqrt");
        private void ExecuteSquare(object? parameter) => ExecuteFunction("sqr");
        private void ExecuteReciprocal(object? parameter) => ExecuteUnaryOperation(calculator.Reciprocal, "1/");
        private void ExecutePercent(object? parameter) => ExecuteUnaryOperation(calculator.Percent, "%");
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
            if (string.IsNullOrWhiteSpace(CurrentInput)) return;

            string trimmed = CurrentInput.Trim();

            // 전체가 이미 음수로 묶여 있는지 검사: -(수식)
            if (trimmed.StartsWith("-(") && trimmed.EndsWith(")"))
            {
                // 음수 해제
                CurrentInput = trimmed.Substring(2, trimmed.Length - 3);
            }
            else
            {
                // 수식 전체를 음수로 감싸기
                CurrentInput = $"-({trimmed})";
            }
            Display = CurrentInput;
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
                    //숫자, 연산자, 괄호, 소수점, 공백만 남기고 나머지는 제거
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

        // Function executers
        private void ExecuteRound(object? parameter) => ExecuteFunction("round");
        private void ExecuteFloor(object? parameter) => ExecuteFunction("floor");
        private void ExecuteCeil(object? parameter) => ExecuteFunction("ceil");
        private void ExecuteAbs(object? parameter) => ExecuteFunction("abs");
        private void ExecuteSign(object? parameter) => ExecuteFunction("sign");
        private void ExecuteSin(object? parameter) => ExecuteFunction("sin");
        private void ExecuteCos(object? parameter) => ExecuteFunction("cos");
        private void ExecuteTan(object? parameter) => ExecuteFunction("tan");
        private void ExecuteAsin(object? parameter) => ExecuteFunction("asin");
        private void ExecuteAcos(object? parameter) => ExecuteFunction("acos");
        private void ExecuteAtan(object? parameter) => ExecuteFunction("atan");
        private void ExecuteLog(object? parameter) => ExecuteFunction("log");
        private void ExecuteLn(object? parameter) => ExecuteFunction("ln");
        private void ExecuteExp(object? parameter) => ExecuteFunction("exp");

        private void ExecutePow(object? parameter)
        {
            if (isResultDisplayed) isResultDisplayed = false;
            if (!string.IsNullOrEmpty(CurrentInput))
            {
                CurrentInput += " ^ ";
                Display = CurrentInput;
            }
        }

        private void ExecutePi(object? parameter)
        {
            if (isResultDisplayed)
            {
                CurrentInput = string.Empty;
                isResultDisplayed = false;
            }

            InsertConstant(Math.PI.ToString());
        }

        private void ExecuteE(object? parameter)
        {
            if (isResultDisplayed)
            {
                CurrentInput = string.Empty;
                isResultDisplayed = false;
            }

            InsertConstant(Math.E.ToString());
        }

        // 공통 메서드
        private void InsertConstant(string constant)
        {
            if (string.IsNullOrWhiteSpace(CurrentInput))
            {
                CurrentInput = constant;
            }
            else
            {
                var tokens = CurrentInput.Trim().Split(' ');
                string lastToken = tokens[^1];

                if (decimal.TryParse(lastToken, out _))
                {
                    // 마지막이 숫자면 제거
                    tokens[^1] = constant;
                    CurrentInput = string.Join(" ", tokens);
                }
                else
                {
                    CurrentInput += " " + constant;
                }
            }

            Display = CurrentInput;
            isResultDisplayed = true; // 상수 입력 후 결과 상태로 설정
        }


        private void ExecuteToggleAngleMode(object? parameter)
        {
            IsRadianMode = !IsRadianMode;
        }

        // Helper methods
        private void ExecuteFunction(string functionName)
        {
            if (string.IsNullOrEmpty(CurrentInput)) return;

            if (isResultDisplayed)
            {
                isResultDisplayed = false;
            }

            // 숫자 주위에 함수 씌우기
            string expr = $"{functionName}({CurrentInput.Replace(" ", "")})";
            CurrentInput = expr;
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

                // Add to history
                historyProvider.Add($"{operatorSymbol}({expression})", result);

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
