using Calculator.Core.Models;
using Calculator.UI.Views;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace Calculator.UI.ViewModels
{
    public class BitOperationsViewModel : INotifyPropertyChanged, IHistoryProvider
    {
        private long operandA = 0;
        private long operandB = 0;
        private long result = 0;
        private string operandAInput = "0";
        private string operandBInput = "0";
        private HistoryWindow? historyWindow;

        // History implementation
        private readonly ObservableCollection<HistoryItem> historyItems;
        public ObservableCollection<HistoryItem> HistoryItems => historyItems;

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

        // Binary representations
        public string OperandABinary => Convert.ToString(operandA, 2).PadLeft(32, '0');
        public string OperandBBinary => Convert.ToString(operandB, 2).PadLeft(32, '0');
        public string ResultBinary => Convert.ToString(result, 2).PadLeft(32, '0');

        // Hex representations
        public string OperandAHex => operandA.ToString("X8");
        public string OperandBHex => operandB.ToString("X8");
        public string ResultHex => result.ToString("X8");

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
            historyItems = new ObservableCollection<HistoryItem>();
            InitializeCommands();
            OpenMenuCommand = new RelayCommand(OpenMenu);
        }

        // IHistoryProvider implementation
        public void AddHistoryItem(string expression, CalcValue result)
        {
            historyItems.Add(new HistoryItem
            {
                Expression = expression,
                Result = result,
                Timestamp = DateTime.Now
            });
        }

        public void ClearHistory()
        {
            historyItems.Clear();
        }

        public void RemoveHistoryItem(HistoryItem item)
        {
            historyItems.Remove(item);
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

        private void ExecuteAnd(object? parameter)
        {
            result = operandA & operandB;
            CurrentOperation = "AND";
            UpdateResultOutputs();
            AddHistoryItem($"{operandA} AND {operandB} = {result}", CalcValue.Create(result));
        }

        private void ExecuteOr(object? parameter)
        {
            result = operandA | operandB;
            CurrentOperation = "OR";
            UpdateResultOutputs();
            AddHistoryItem($"{operandA} OR {operandB} = {result}", CalcValue.Create(result));
        }

        private void ExecuteXor(object? parameter)
        {
            result = operandA ^ operandB;
            CurrentOperation = "XOR";
            UpdateResultOutputs();
            AddHistoryItem($"{operandA} XOR {operandB} = {result}", CalcValue.Create(result));
        }

        private void ExecuteNotA(object? parameter)
        {
            result = ~operandA;
            CurrentOperation = "NOT A";
            UpdateResultOutputs();
            AddHistoryItem($"NOT {operandA} = {result}", CalcValue.Create(result));
        }

        private void ExecuteNotB(object? parameter)
        {
            result = ~operandB;
            CurrentOperation = "NOT B";
            UpdateResultOutputs();
            AddHistoryItem($"NOT {operandB} = {result}", CalcValue.Create(result));
        }

        private void ExecuteLeftShift(object? parameter)
        {
            if (operandB >= 0 && operandB < 64)
            {
                result = operandA << (int)operandB;
                CurrentOperation = $"A << {operandB}";
                UpdateResultOutputs();
                AddHistoryItem($"{operandA} << {operandB} = {result}", CalcValue.Create(result));
            }
        }

        private void ExecuteRightShift(object? parameter)
        {
            if (operandB >= 0 && operandB < 64)
            {
                result = operandA >> (int)operandB;
                CurrentOperation = $"A >> {operandB}";
                UpdateResultOutputs();
                AddHistoryItem($"{operandA} >> {operandB} = {result}", CalcValue.Create(result));
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
            AddHistoryItem($"Swapped operands: A={operandA}, B={operandB}", CalcValue.Create(0));
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
            // 현재 연산이 있다면 다시 계산
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
            ClearHistory();
        }

        private void ExecuteShowHistory(object? parameter)
        {
            // 이미 열려있는 창이 있으면 앞으로 가져오기
            if (historyWindow != null)
            {
                historyWindow.Activate();
                historyWindow.Focus();
                return;
            }

            // 새 히스토리 창 생성 (모달리스)
            historyWindow = new HistoryWindow(this);
            historyWindow.Owner = Application.Current.MainWindow;
            historyWindow.Closed += (s, e) => historyWindow = null; // 창이 닫히면 참조 해제
            historyWindow.Show(); // ShowDialog() 대신 Show() 사용
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }