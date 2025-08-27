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
                operandAInput = value ?? "0";
                OnPropertyChanged();
                UpdateOperandA();
                UpdateResults();
            }
        }

        public string OperandBInput
        {
            get => operandBInput;
            set
            {
                operandBInput = value ?? "0";
                OnPropertyChanged();
                UpdateOperandB();
                UpdateResults();
            }
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
        public ICommand? OpenMenuCommand { get; }
        public ICommand? AndCommand { get; private set; }
        public ICommand? OrCommand { get; private set; }
        public ICommand? XorCommand { get; private set; }
        public ICommand? NotACommand { get; private set; }
        public ICommand? NotBCommand { get; private set; }
        public ICommand? LeftShiftCommand { get; private set; }
        public ICommand? RightShiftCommand { get; private set; }
        public ICommand? ClearCommand { get; private set; }
        public ICommand? SwapCommand { get; private set; }
        public ICommand? ClearHistoryCommand { get; private set; }
        public ICommand? ShowHistoryCommand { get; private set; }

        public BitOperationsViewModel()
        {
            historyProvider = App.HistoryService ?? new HistoryService();
            InitializeCommands();
            OpenMenuCommand = new RelayCommand(OpenMenu);
        }

        private void InitializeCommands()
        {
            AndCommand = new RelayCommand(ExecuteAnd);
            OrCommand = new RelayCommand(ExecuteOr);
            XorCommand = new RelayCommand(ExecuteXor);
            NotACommand = new RelayCommand(ExecuteNotA);
            NotBCommand = new RelayCommand(ExecuteNotB);
            LeftShiftCommand = new RelayCommand(ExecuteLeftShift);
            RightShiftCommand = new RelayCommand(ExecuteRightShift);
            ClearCommand = new RelayCommand(ExecuteClear);
            SwapCommand = new RelayCommand(ExecuteSwap);
            ShowHistoryCommand = new RelayCommand(ExecuteShowHistory);
            ClearHistoryCommand = new RelayCommand(ExecuteClearHistory);
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

        private void ExecuteSwap(object? parameter)
        {
            (OperandAInput, OperandBInput) = (OperandBInput, OperandAInput);
            historyProvider.Add($"Swapped operands: A={operandA}, B={operandB}", 0);
        }

        private void OpenMenu(object? parameter)
        {
            if (parameter is Window currentWindow)
            {
                var menuWindow = new MenuWindow();
                menuWindow.Show();

                currentWindow.Close();
            }
        }

        private void UpdateOperandA()
        {
            try
            {
                if (long.TryParse(operandAInput, out long value))
                    operandA = value;
                else if (operandAInput.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                    operandA = Convert.ToInt64(operandAInput, 16);
                else if (operandAInput.StartsWith("0b", StringComparison.OrdinalIgnoreCase))
                    operandA = Convert.ToInt64(operandAInput.Substring(2), 2);
                else
                    operandA = 0;
            }
            catch
            {
                operandA = 0;
            }

            OnPropertyChanged(nameof(OperandABinary));
            OnPropertyChanged(nameof(OperandAHex));
            OnPropertyChanged(nameof(OperandADecimal));
        }

        private void UpdateOperandB()
        {
            try
            {
                if (long.TryParse(operandBInput, out long value))
                    operandB = value;
                else if (operandBInput.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                    operandB = Convert.ToInt64(operandBInput, 16);
                else if (operandBInput.StartsWith("0b", StringComparison.OrdinalIgnoreCase))
                    operandB = Convert.ToInt64(operandBInput.Substring(2), 2);
                else
                    operandB = 0;
            }
            catch
            {
                operandB = 0;
            }

            OnPropertyChanged(nameof(OperandBBinary));
            OnPropertyChanged(nameof(OperandBHex));
            OnPropertyChanged(nameof(OperandBDecimal));
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
                case "NOT A":
                    result = ~operandA;
                    break;
                case "NOT B":
                    result = ~operandB;
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