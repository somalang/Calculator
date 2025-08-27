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
    public class BaseConverterViewModel : INotifyPropertyChanged
    {
        private string inputValue = "0";
        private int currentBase = 10;
        private long decimalValue = 0;
        private string display = string.Empty;

        private HistoryWindow? historyWindow;

        private readonly IHistoryProvider historyProvider;

        // HistoryItems는 외부의 HistoryService에서 가져옴
        public ObservableCollection<HistoryItem> HistoryItems => historyProvider.Items;

        public string InputValue
        {
            get => inputValue;
            set
            {
                inputValue = value ?? "0";
                OnPropertyChanged();
                UpdateDecimalValue();
                UpdateOutputs();
            }
        }
        public string Display
        {
            get => display;
            set
            {
                display = value;
                OnPropertyChanged();
            }
        }

        // Input base properties
        public bool IsBinaryInput
        {
            get => currentBase == 2;
            set
            {
                if (value)
                {
                    currentBase = 2;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(IsOctalInput));
                    OnPropertyChanged(nameof(IsDecimalInput));
                    OnPropertyChanged(nameof(IsHexInput));
                    UpdateDigitAvailability();
                    UpdateDecimalValue();
                    UpdateOutputs();
                }
            }
        }

        public bool IsOctalInput
        {
            get => currentBase == 8;
            set
            {
                if (value)
                {
                    currentBase = 8;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(IsBinaryInput));
                    OnPropertyChanged(nameof(IsDecimalInput));
                    OnPropertyChanged(nameof(IsHexInput));
                    UpdateDigitAvailability();
                    UpdateDecimalValue();
                    UpdateOutputs();
                }
            }
        }

        public bool IsDecimalInput
        {
            get => currentBase == 10;
            set
            {
                if (value)
                {
                    currentBase = 10;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(IsBinaryInput));
                    OnPropertyChanged(nameof(IsOctalInput));
                    OnPropertyChanged(nameof(IsHexInput));
                    UpdateDigitAvailability();
                    UpdateDecimalValue();
                    UpdateOutputs();
                }
            }
        }

        public bool IsHexInput
        {
            get => currentBase == 16;
            set
            {
                if (value)
                {
                    currentBase = 16;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(IsBinaryInput));
                    OnPropertyChanged(nameof(IsOctalInput));
                    OnPropertyChanged(nameof(IsDecimalInput));
                    UpdateDigitAvailability();
                    UpdateDecimalValue();
                    UpdateOutputs();
                }
            }
        }

        // Digit availability properties
        private bool isBinaryDigitEnabled = true;
        private bool isOctalDigitEnabled = true;
        private bool isDecimalDigitEnabled = true;
        private bool isHexDigitEnabled = true;

        public bool IsBinaryDigitEnabled
        {
            get => isBinaryDigitEnabled;
            set
            {
                isBinaryDigitEnabled = value;
                OnPropertyChanged();
            }
        }

        public bool IsOctalDigitEnabled
        {
            get => isOctalDigitEnabled;
            set
            {
                isOctalDigitEnabled = value;
                OnPropertyChanged();
            }
        }

        public bool IsDecimalDigitEnabled
        {
            get => isDecimalDigitEnabled;
            set
            {
                isDecimalDigitEnabled = value;
                OnPropertyChanged();
            }
        }

        public bool IsHexDigitEnabled
        {
            get => isHexDigitEnabled;
            set
            {
                isHexDigitEnabled = value;
                OnPropertyChanged();
            }
        }

        // Output properties
        public string BinaryOutput => Convert.ToString(decimalValue, 2);
        public string OctalOutput => Convert.ToString(decimalValue, 8);
        public string DecimalOutput => decimalValue.ToString();
        public string HexOutput => Convert.ToString(decimalValue, 16).ToUpper();

        // Commands
        public ICommand? DigitCommand { get; private set; }
        public ICommand? ClearCommand { get; private set; }
        public ICommand? BackspaceCommand { get; private set; }
        public ICommand? CopyCommand { get; private set; }
        public ICommand? PasteCommand { get; private set; }
        public ICommand? OpenMenuCommand { get; }
        public ICommand? ClearHistoryCommand { get; private set; }
        public ICommand? ShowHistoryCommand { get; private set; }
        public ICommand? ToggleSignCommand { get; private set; }

        public BaseConverterViewModel()
        {
            historyProvider = App.HistoryService ?? new HistoryService();
            InitializeCommands();       // 추가

            UpdateDigitAvailability();
            OpenMenuCommand = new RelayCommand(OpenMenu);
        }

        // Remove IHistoryProvider implementation methods since we use HistoryService directly

        private void InitializeCommands()
        {
            DigitCommand = new RelayCommand(ExecuteDigit);
            ClearCommand = new RelayCommand(ExecuteClear);
            BackspaceCommand = new RelayCommand(ExecuteBackspace);
            CopyCommand = new RelayCommand(ExecuteCopy);
            PasteCommand = new RelayCommand(ExecutePaste);
            ShowHistoryCommand = new RelayCommand(ExecuteShowHistory);
            ClearHistoryCommand = new RelayCommand(_ => historyProvider.Clear());
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
        private string GetCurrentBaseString()
        {
            return currentBase switch
            {
                2 => "BIN: ",
                8 => "OCT: ",
                10 => "DEC: ",
                16 => "HEX: ",
                _ => ""
            };
        }


        private void ExecuteDigit(object? parameter)
        {
            string? digit = parameter?.ToString();
            if (string.IsNullOrEmpty(digit)) return;

            string oldValue = InputValue;
            if (InputValue == "0")
                InputValue = digit;
            else
                InputValue += digit;

            // Add conversion to history when input changes
            if (InputValue != oldValue && InputValue != "0")
            {
                AddConversionToHistory();
            }
        }

        private void ExecuteClear(object? parameter)
        {
            InputValue = "0";
        }

        private void ExecuteBackspace(object? parameter)
        {
            if (InputValue.Length > 1)
                InputValue = InputValue.Substring(0, InputValue.Length - 1);
            else
                InputValue = "0";
        }

        private void ExecuteCopy(object? parameter)
        {
            string result = $"BIN: {BinaryOutput}\nOCT: {OctalOutput}\nDEC: {DecimalOutput}\nHEX: {HexOutput}";
            Clipboard.SetText(result);

            // Add copy action to history
            historyProvider.Add($"Copy: {GetCurrentBaseString()}{InputValue}", decimalValue);
        }

        private void ExecutePaste(object? parameter)
        {
            if (Clipboard.ContainsText())
            {
                string clipboardText = Clipboard.GetText().Trim();
                if (IsValidInput(clipboardText))
                {
                    string oldValue = InputValue;
                    InputValue = clipboardText;

                    // Add paste action to history
                    if (InputValue != oldValue)
                    {
                        historyProvider.Add($"Paste: {GetCurrentBaseString()}{InputValue}", decimalValue);
                    }
                }
            }
        }

        private void ExecuteToggleSign(object? parameter)
        {
            // Toggle sign functionality if needed
        }

        private void UpdateDigitAvailability()
        {
            IsBinaryDigitEnabled = currentBase >= 2;
            IsOctalDigitEnabled = currentBase >= 8;
            IsDecimalDigitEnabled = currentBase >= 10;
            IsHexDigitEnabled = currentBase >= 16;
        }

        private void UpdateDecimalValue()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(InputValue) || InputValue == "0")
                {
                    decimalValue = 0;
                    return;
                }

                decimalValue = Convert.ToInt64(InputValue, currentBase);
            }
            catch
            {
                decimalValue = 0;
            }
        }

        private void UpdateOutputs()
        {
            OnPropertyChanged(nameof(BinaryOutput));
            OnPropertyChanged(nameof(OctalOutput));
            OnPropertyChanged(nameof(DecimalOutput));
            OnPropertyChanged(nameof(HexOutput));
        }

        private bool IsValidInput(string input)
        {
            try
            {
                Convert.ToInt64(input, currentBase);
                return true;
            }
            catch
            {
                return false;
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

            // HistoryViewModel 생성 후 DataContext에 연결
            var historyViewModel = new HistoryViewModel(
                historyProvider,
                expression =>
                {
                    inputValue = expression;
                    Display = expression;
                    //isResultDisplayed = false;
                });
           
            historyWindow.DataContext = historyViewModel; // 중요: ViewModel 바인딩
            historyWindow.Owner = Application.Current.MainWindow;
            historyWindow.Closed += (s, e) => historyWindow = null;
            historyWindow.Show();
        }



        private void ExecuteClearHistory(object? parameter)
        {
            // HistoryViewModel을 거치지 않고 historyProvider 직접 Clear
            historyProvider.Clear();
        }

        // Input 변경 시 히스토리 추가
        private void AddConversionToHistory()
        {
            string baseString = GetCurrentBaseString();
            string expression = $"{baseString}{InputValue} = DEC: {DecimalOutput}, BIN: {BinaryOutput}, OCT: {OctalOutput}, HEX: {HexOutput}";

            // HistoryViewModel을 거치지 않고 historyProvider에 추가
            historyProvider.Add(expression, decimalValue);
        }


        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}