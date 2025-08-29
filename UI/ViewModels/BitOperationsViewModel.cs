using Calculator.Core.Models;
using Calculator.Core.Services; // IHistoryProvider 사용
using Calculator.UI.Views;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace Calculator.UI.ViewModels
{
    public class BitOperationsViewModel : INotifyPropertyChanged
    {
        private long operandA = 0;
        private long operandB = 0;
        private long result = 0;
        private string operandAInput = "0";
        private string operandBInput = "0";
        private bool isOperandAFocused = false;
        private bool isOperandBFocused = false;
        private HistoryWindow? historyWindow;

        private readonly IHistoryProvider historyProvider;

        // 외부에서 History 참조 가능
        public ObservableCollection<HistoryItem> HistoryItems => historyProvider.Items;

        public string OperandAInput
        {
            get => operandAInput;
            set
            {
                if (IsValidNumericInput(value))
                {
                    operandAInput = value ?? ""; // 빈 문자열 허용
                    OnPropertyChanged();
                    UpdateOperandA();
                    UpdateResults();
                }
                else
                {
                    // 잘못된 입력은 무시 (기존 값 유지)
                }
            }
        }

        public string OperandBInput
        {
            get => operandBInput;
            set
            {
                if (IsValidNumericInput(value))
                {
                    operandBInput = value ?? "";
                    OnPropertyChanged();
                    UpdateOperandB();
                    UpdateResults();
                }
                else
                {
                    // 잘못된 입력은 무시
                }
            }
        }

        /// <summary>
        /// 숫자 또는 음수(-)만 허용
        /// </summary>
        private bool IsValidNumericInput(string? input)
        {
            if (string.IsNullOrEmpty(input)) return true; // 빈 입력 허용
            return System.Text.RegularExpressions.Regex.IsMatch(input, @"^-?\d+$");
        }


        public bool IsOperandAFocused
        {
            get => isOperandAFocused;
            set
            {
                isOperandAFocused = value;
                OnPropertyChanged();
            }
        }

        public bool IsOperandBFocused
        {
            get => isOperandBFocused;
            set
            {
                isOperandBFocused = value;
                OnPropertyChanged();
            }
        }

        // Binary representations (16비트로 단축)
        public string OperandABinary => GetBinaryString(operandA);
        public string OperandBBinary => GetBinaryString(operandB);
        public string ResultBinary => GetBinaryString(result);

        // Hex representations
        public string OperandAHex => operandA.ToString("X4");
        public string OperandBHex => operandB.ToString("X4");
        public string ResultHex => result.ToString("X4");

        // Decimal representations
        public string OperandADecimal => operandA.ToString();
        public string OperandBDecimal => operandB.ToString();
        public string ResultDecimal => result.ToString();

        // Current operation
        private string currentOperation = "None";
        public string CurrentOperation
        {
            get => currentOperation;
            set
            {
                currentOperation = value;
                OnPropertyChanged();
            }
        }

        // Commands
        public ICommand? CloseCommand { get; }
        public ICommand? OpenMenuCommand { get; private set; }
        public ICommand? AndCommand { get; private set; }
        public ICommand? OrCommand { get; private set; }
        public ICommand? XorCommand { get; private set; }
        public ICommand? NandCommand { get; private set; }  // 추가
        public ICommand? NorCommand { get; private set; }   // 추가
        public ICommand? NxorCommand { get; private set; }  // 추가
        public ICommand? NotACommand { get; private set; }
        public ICommand? NotBCommand { get; private set; }
        public ICommand? LeftShiftCommand { get; private set; }
        public ICommand? RightShiftCommand { get; private set; }
        public ICommand? ClearCommand { get; private set; }
        public ICommand? ClearACommand { get; private set; }  // 추가
        public ICommand? ClearBCommand { get; private set; }  // 추가
        public ICommand? SwapCommand { get; private set; }
        public ICommand? ClearHistoryCommand { get; private set; }
        public ICommand? ShowHistoryCommand { get; private set; }
        public ICommand? NumberCommand { get; private set; }
        public ICommand? BackspaceCommand { get; private set; }
        public ICommand? ClearEntryCommand { get; private set; }
        public ICommand? ToggleSignCommand { get; private set; }  // 추가

        public BitOperationsViewModel()
        {
            historyProvider = App.HistoryService ?? new HistoryService();
            InitializeCommands();
            CloseCommand = new RelayCommand(CloseWindow);
            OpenMenuCommand = new RelayCommand(OpenMenu);

        }

        private void InitializeCommands()
        {
            AndCommand = new RelayCommand(ExecuteAnd);
            OrCommand = new RelayCommand(ExecuteOr);
            XorCommand = new RelayCommand(ExecuteXor);
            NandCommand = new RelayCommand(ExecuteNand);  // 추가
            NorCommand = new RelayCommand(ExecuteNor);    // 추가
            NxorCommand = new RelayCommand(ExecuteNxor);  // 추가
            NotACommand = new RelayCommand(ExecuteNotA);
            NotBCommand = new RelayCommand(ExecuteNotB);
            LeftShiftCommand = new RelayCommand(ExecuteLeftShift);
            RightShiftCommand = new RelayCommand(ExecuteRightShift);
            ClearCommand = new RelayCommand(ExecuteClear);
            ClearACommand = new RelayCommand(ExecuteClearA);  // 추가
            ClearBCommand = new RelayCommand(ExecuteClearB);  // 추가
            SwapCommand = new RelayCommand(ExecuteSwap);
            ShowHistoryCommand = new RelayCommand(ExecuteShowHistory);
            ClearHistoryCommand = new RelayCommand(ExecuteClearHistory);
            BackspaceCommand = new RelayCommand(ExecuteBackspace);
            NumberCommand = new RelayCommand(ExecuteNumber);
            ToggleSignCommand = new RelayCommand(ExecuteToggleSign);  // 추가
        }

        private string GetBinaryString(long value)
        {
            // 값에 따라 적절한 길이로 표시
            if (value == 0) return "0";

            string binary = Convert.ToString(value, 2);

            // 8비트 단위로 그룹화하되, 최소 8비트, 최대 16비트
            int targetLength = binary.Length <= 8 ? 8 : 16;
            return binary.PadLeft(targetLength, '0');
        }

        // Focus 처리 메서드들 (코드비하인드에서 호출용)
        public void HandleOperandAGotFocus()
        {
            IsOperandAFocused = true;
            if (operandAInput == "0")
            {
                OperandAInput = "";
            }
        }

        public void HandleOperandALostFocus()
        {
            IsOperandAFocused = false;
            if (string.IsNullOrWhiteSpace(operandAInput))
            {
                OperandAInput = "0";
            }
        }

        public void HandleOperandBGotFocus()
        {
            IsOperandBFocused = true;
            if (operandBInput == "0")
            {
                OperandBInput = "";
            }
        }

        public void HandleOperandBLostFocus()
        {
            IsOperandBFocused = false;
            if (string.IsNullOrWhiteSpace(operandBInput))
            {
                OperandBInput = "0";
            }
        }
        private void CloseWindow(object? parameter)
        {
            // 현재 열려 있는 창을 닫음
            Application.Current.Windows
                .OfType<Window>()
                .FirstOrDefault(w => w.DataContext == this)
                ?.Close();
        }
        private void ExecuteNumber(object? parameter)
        {
            var digit = parameter?.ToString();
            if (string.IsNullOrEmpty(digit))
                return;

            if (IsOperandAFocused)
            {
                if (OperandAInput == "0") OperandAInput = "";
                OperandAInput += digit;
            }
            else if (IsOperandBFocused)
            {
                if (OperandBInput == "0") OperandBInput = "";
                OperandBInput += digit;
            }
            else
            {
                // 아무 데도 포커스가 없으면 기본적으로 A에 입력
                if (OperandAInput == "0") OperandAInput = "";
                OperandAInput += digit;
            }
        }

        private void ExecuteToggleSign(object? parameter)
        {
            if (IsOperandAFocused)
            {
                if (long.TryParse(OperandAInput, out long value))
                {
                    OperandAInput = (-value).ToString();
                }
            }
            else if (IsOperandBFocused)
            {
                if (long.TryParse(OperandBInput, out long value))
                {
                    OperandBInput = (-value).ToString();
                }
            }
            else
            {
                // 기본적으로 A 값 변경
                if (long.TryParse(OperandAInput, out long value))
                {
                    OperandAInput = (-value).ToString();
                }
            }
        }

        private void ExecuteAnd(object? parameter)
        {
            result = operandA & operandB;
            CurrentOperation = "AND";
            UpdateResultOutputs();
            historyProvider.Add($"{operandA} & {operandB} = {result}", result);
        }

        private void ExecuteOr(object? parameter)
        {
            result = operandA | operandB;
            CurrentOperation = "OR";
            UpdateResultOutputs();
            historyProvider.Add($"{operandA} | {operandB} = {result}", result);
        }

        private void ExecuteXor(object? parameter)
        {
            result = operandA ^ operandB;
            CurrentOperation = "XOR";
            UpdateResultOutputs();
            historyProvider.Add($"{operandA} ^ {operandB} = {result}", result);
        }

        // 새로 추가된 연산들
        private void ExecuteNand(object? parameter)
        {
            result = ~(operandA & operandB);
            CurrentOperation = "NAND";
            UpdateResultOutputs();
            historyProvider.Add($"~({operandA} & {operandB}) = {result}", result);
        }

        private void ExecuteNor(object? parameter)
        {
            result = ~(operandA | operandB);
            CurrentOperation = "NOR";
            UpdateResultOutputs();
            historyProvider.Add($"~({operandA} | {operandB}) = {result}", result);
        }

        private void ExecuteNxor(object? parameter)
        {
            result = ~(operandA ^ operandB);
            CurrentOperation = "NXOR";
            UpdateResultOutputs();
            historyProvider.Add($"~({operandA} ^ {operandB}) = {result}", result);
        }

        private void ExecuteNotA(object? parameter)
        {
            result = ~operandA;
            CurrentOperation = "NOT A";
            UpdateResultOutputs();
            historyProvider.Add($"NOT{operandA} = {result}", result);
        }

        private void ExecuteNotB(object? parameter)
        {
            result = ~operandB;
            CurrentOperation = "NOT B";
            UpdateResultOutputs();
            historyProvider.Add($"NOT{operandB} = {result}", result);
        }

        private void ExecuteLeftShift(object? parameter)
        {
            if (operandB >= 0 && operandB < 64)
            {
                result = operandA << (int)operandB;
                CurrentOperation = $"A << {operandB}";
                UpdateResultOutputs();
                historyProvider.Add($"{operandA} << {operandB} = {result}", result);
            }
        }

        private void ExecuteRightShift(object? parameter)
        {
            if (operandB >= 0 && operandB < 64)
            {
                result = operandA >> (int)operandB;
                CurrentOperation = $"A >> {operandB}";
                UpdateResultOutputs();
                historyProvider.Add($"{operandA} >> {operandB} = {result}", result);
            }
        }

        private void ExecuteClear(object? parameter)
        {
            OperandAInput = "0";
            OperandBInput = "0";
            result = 0;
            CurrentOperation = "None";
            UpdateResultOutputs();
        }

        // 새로 추가된 Clear 버튼들
        private void ExecuteClearA(object? parameter)
        {
            OperandAInput = "0";
            IsOperandAFocused = true;  // 포커스 B로 이동
            historyProvider.Add("Cleared Operand A", 0);
        }

        private void ExecuteClearB(object? parameter)
        {
            OperandBInput = "0";
            IsOperandBFocused = true;  // 포커스 B로 이동
            historyProvider.Add("Cleared Operand B", 0);
        }

        private void ExecuteBackspace(object? parameter)
        {
            if (IsOperandAFocused)
            {
                if (OperandAInput.Length > 0)
                    OperandAInput = OperandAInput[..^1];
                if (string.IsNullOrEmpty(OperandAInput))
                    OperandAInput = "0";
            }
            else if (IsOperandBFocused)
            {
                if (OperandBInput.Length > 0)
                    OperandBInput = OperandBInput[..^1];
                if (string.IsNullOrEmpty(OperandBInput))
                    OperandBInput = "0";
            }
        }

        private void ExecuteSwap(object? parameter)
        {
            (OperandAInput, OperandBInput) = (OperandBInput, OperandAInput);
            historyProvider.Add($"Swapped operands: A={operandA}, B={operandB}", 0);
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


        private void UpdateOperandA()
        {
            if (long.TryParse(operandAInput, out long result))
                operandA = result;
            else
                operandA = 0; // 빈 입력이나 잘못된 값 → 0 처리
        }

        private void UpdateOperandB()
        {
            if (long.TryParse(operandBInput, out long result))
                operandB = result;
            else
                operandB = 0;
        }


        private void UpdateResults()
        {
            switch (CurrentOperation)
            {
                case "AND":
                    result = operandA & operandB;
                    break;
                case "OR":
                    result = operandA | operandB;
                    break;
                case "XOR":
                    result = operandA ^ operandB;
                    break;
                case "NAND":
                    result = ~(operandA & operandB);
                    break;
                case "NOR":
                    result = ~(operandA | operandB);
                    break;
                case "NXOR":
                    result = ~(operandA ^ operandB);
                    break;
                case "NOT A":
                    result = ~operandA;
                    break;
                case "NOT B":
                    result = ~operandB;
                    break;
                case var op when op.StartsWith("A <<"):
                    if (operandB >= 0 && operandB < 64)
                        result = operandA << (int)operandB;
                    break;
                case var op when op.StartsWith("A >>"):
                    if (operandB >= 0 && operandB < 64)
                        result = operandA >> (int)operandB;
                    break;
                default:
                    result = 0;
                    break;
            }
            UpdateResultOutputs();
        }


        private void UpdateResultOutputs()
        {
            OnPropertyChanged(nameof(ResultBinary));
            OnPropertyChanged(nameof(ResultHex));
            OnPropertyChanged(nameof(ResultDecimal));
        }

        private void ExecuteClearHistory(object? parameter)
        {
            historyProvider.Clear();
        }

        private void ExecuteShowHistory(object? parameter)
        {
            if (historyWindow != null)
            {
                historyWindow.Activate();
                return;
            }

            var historyViewModel = new HistoryViewModel(historyProvider, ParseAndSetOperands);
            historyWindow = new HistoryWindow { DataContext = historyViewModel };
            historyWindow.Show();
        }

        private void ParseAndSetOperands(string expression)
        {
            // 숫자 추출
            var match = System.Text.RegularExpressions.Regex.Match(expression, @"-?\d+");
            if (match.Success)
            {
                OperandAInput = match.Value;
            }
            else
            {
                OperandAInput = "0";
            }

            // NOT 연산이면 B는 0으로 초기화
            if (expression.StartsWith("~") || expression.StartsWith("NOT"))
                OperandBInput = "0";
            else
            {
                // 이항 연산 처리
                var numbers = System.Text.RegularExpressions.Regex.Matches(expression, @"-?\d+")
                                  .Cast<System.Text.RegularExpressions.Match>()
                                  .Select(m => m.Value)
                                  .ToArray();
                if (numbers.Length >= 2)
                {
                    OperandAInput = numbers[0];
                    OperandBInput = numbers[1];
                }
                else if (numbers.Length == 1)
                {
                    OperandAInput = numbers[0];
                    OperandBInput = "0";
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
