using Calculator.UI.Views;
using System;
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

        public BaseConverterViewModel()
        {
            InitializeCommands();
            UpdateDigitAvailability();
            OpenMenuCommand = new RelayCommand(OpenMenu);
        }

        private void InitializeCommands()
        {
            DigitCommand = new RelayCommand(ExecuteDigit);
            ClearCommand = new RelayCommand(ExecuteClear);
            BackspaceCommand = new RelayCommand(ExecuteBackspace);
            CopyCommand = new RelayCommand(ExecuteCopy);
            PasteCommand = new RelayCommand(ExecutePaste);
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

        private void ExecuteDigit(object parameter)
        {
            string digit = parameter?.ToString();
            if (string.IsNullOrEmpty(digit)) return;

            if (InputValue == "0")
                InputValue = digit;
            else
                InputValue += digit;
        }

        private void ExecuteClear(object parameter)
        {
            InputValue = "0";
        }

        private void ExecuteBackspace(object parameter)
        {
            if (InputValue.Length > 1)
                InputValue = InputValue.Substring(0, InputValue.Length - 1);
            else
                InputValue = "0";
        }

        private void ExecuteCopy(object parameter)
        {
            string result = $"BIN: {BinaryOutput}\nOCT: {OctalOutput}\nDEC: {DecimalOutput}\nHEX: {HexOutput}";
            Clipboard.SetText(result);
        }

        private void ExecutePaste(object parameter)
        {
            if (Clipboard.ContainsText())
            {
                string clipboardText = Clipboard.GetText().Trim();
                if (IsValidInput(clipboardText))
                    InputValue = clipboardText;
            }
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

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}