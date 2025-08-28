using Calculator.Core.Models;
using Calculator.Core.Services;
using Calculator.UI.Views;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Text.RegularExpressions; // 파일 상단에 추가

namespace Calculator.UI.ViewModels
{
    public class BaseConverterViewModel : INotifyPropertyChanged
    {
        private string inputValue = string.Empty;
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
                // 들어온 값이 유효하지 않으면 아무것도 하지 않고 즉시 종료합니다.
                if (!IsValidInput(value))
                {
                    return;
                }

                // 유효한 값일 경우에만 아래 로직을 실행합니다.
                string? newInputValue = value;
                if (inputValue == "0" && !string.IsNullOrEmpty(newInputValue) && newInputValue != "0")
                {
                    inputValue = newInputValue;
                }
                else
                {
                    inputValue = newInputValue ?? "0";
                }

                OnPropertyChanged();
                UpdateDecimalValue();
                UpdateOutputs();
            }
        }

        private bool IsValidInput(string? input)
        {
            if (string.IsNullOrEmpty(input)) return true; // 빈 입력은 유효한 것으로 처리

            // 허용되지 않는 문자가 포함되어 있으면 false를 반환합니다.
            return !System.Text.RegularExpressions.Regex.IsMatch(input, @"[^0-9a-fA-F]");
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
        public ICommand? KeyPressCommand { get; private set; }
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
            InitializeCommands();

            // 초기 값을 "0"으로 설정
            display = "0";
            inputValue = string.Empty;
            UpdateDigitAvailability();
            OpenMenuCommand = new RelayCommand(OpenMenu);
        }

        // Remove IHistoryProvider implementation methods since we use HistoryService directly

        private void InitializeCommands()
        {
            KeyPressCommand = new RelayCommand(ExecuteKeyPress);
            DigitCommand = new RelayCommand(ExecuteDigit);
            ClearCommand = new RelayCommand(ExecuteClear);
            BackspaceCommand = new RelayCommand(ExecuteBackspace);
            CopyCommand = new RelayCommand(ExecuteCopy);
            PasteCommand = new RelayCommand(ExecutePaste);
            ShowHistoryCommand = new RelayCommand(ExecuteShowHistory);
            ClearHistoryCommand = new RelayCommand(ExecuteClearHistory);
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

        private void ExecuteKeyPress(object? parameter)
        {
            // View에서 전달된 KeyEventArgs를 받아 처리
            if (parameter is KeyEventArgs keyArgs)
            {
                HandleKeyPress(keyArgs.Key);
            }
        }

        private void HandleKeyPress(Key key)
        {
            // 백스페이스와 삭제 키 먼저 처리
            if (key == Key.Back) { ExecuteBackspace(null); return; }
            if (key == Key.Delete) { ExecuteClear(null); return; }

            // 키를 문자로 변환 (숫자 0-9, 알파벳 A-F)
            string digit = string.Empty;
            if (key >= Key.D0 && key <= Key.D9)
                digit = ((char)('0' + (key - Key.D0))).ToString();
            else if (key >= Key.NumPad0 && key <= Key.NumPad9)
                digit = ((char)('0' + (key - Key.NumPad0))).ToString();
            else if (key >= Key.A && key <= Key.F)
                digit = key.ToString();

            // 유효한 입력이 아니면 무시
            if (string.IsNullOrEmpty(digit)) return;

            // 현재 진법에 문자가 유효한지 확인
            char c = digit[0];
            int val = (c >= 'A') ? (10 + c - 'A') : (c - '0');

            if (val < currentBase)
            {
                // 유효하면 숫자 입력 처리 메서드 호출
                ExecuteDigit(digit);
            }
        }
        private void ExecuteDigit(object? parameter)
        {
            string? digit = parameter?.ToString();
            if (string.IsNullOrEmpty(digit)) return;

            // '0'이 아닌 다른 숫자가 입력되면 기존의 '0'을 대체
            if (inputValue == "0" && digit != "0")
            {
                InputValue = digit;
            }
            // 기존 값에 새로운 숫자 추가
            else
            {
                InputValue += digit;
            }

            // 히스토리에 변환 결과 추가
            AddConversionToHistory();
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
        }

        private void ExecutePaste(object? parameter)
        {
            if (Clipboard.ContainsText())
            {
                string clipboardText = Clipboard.GetText().Trim();

                // 현재 진법에 따라 허용할 문자 패턴을 결정
                string pattern = currentBase switch
                {
                    2 => @"[^0-1]",
                    8 => @"[^0-7]",
                    10 => @"[^0-9]",
                    16 => @"[^0-9a-fA-F]",
                    _ => @"[\s\S]" // 그 외의 경우 모든 문자를 제거
                };

                // 정규식을 사용해 허용된 문자만 남김
                string filteredText = Regex.Replace(clipboardText, pattern, string.Empty);

                if (!string.IsNullOrEmpty(filteredText))
                {
                    InputValue = filteredText;
                    historyProvider.Add($"Paste: {GetCurrentBaseString()}{InputValue}", decimalValue);
                }
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

        private void ExecuteShowHistory(object? parameter)
        {
            // 이미 열려있는 히스토리 창이 있으면 활성화
            if (historyWindow != null)
            {
                historyWindow.Activate();
                historyWindow.Focus();
                return;
            }

            try
            {
                // HistoryViewModel 생성 시 선택 시 호출할 액션 전달
                var historyViewModel = new HistoryViewModel(
                    historyProvider,
                    expression =>
                    {
                        // "BIN: 1010 => ..." 형태에서 "1010" 부분만 추출
                        var match = Regex.Match(expression, @"^(BIN|OCT|DEC|HEX): (.+?) =>");
                        if (match.Success)
                        {
                            string inputValue = match.Groups[2].Value.Trim();
                            InputValue = inputValue;
                            Display = $"{GetCurrentBaseString()}{inputValue}";
                        }
                    });

                // HistoryWindow 생성 및 연결
                historyWindow = new HistoryWindow();
                historyWindow.DataContext = historyViewModel;
                historyWindow.Owner = Application.Current.MainWindow;
                historyWindow.Closed += (s, e) => historyWindow = null;
                historyWindow.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"히스토리 창을 열 수 없습니다: {ex.Message}", "오류",
                                MessageBoxButton.OK, MessageBoxImage.Error);
                historyWindow = null;
            }
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
            string expression = $"{baseString}{InputValue} => DEC: {DecimalOutput}, BIN: {BinaryOutput}, OCT: {OctalOutput}, HEX: {HexOutput}";

            // HistoryViewModel을 거치지 않고 historyProvider에 추가
            historyProvider.Add(expression, decimalValue);
        }


        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
