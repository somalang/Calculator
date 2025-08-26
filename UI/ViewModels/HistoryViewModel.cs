using Calculator.Core.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace Calculator.UI.ViewModels
{
    public class HistoryViewModel : INotifyPropertyChanged
    {
        private readonly IHistoryProvider historyProvider;
        private readonly Window window;
        private HistoryItem? selectedHistoryItem;

        // IHistoryProvider의 HistoryItems를 직접 참조
        public ObservableCollection<HistoryItem> HistoryItems => historyProvider.HistoryItems;

        public HistoryItem? SelectedHistoryItem
        {
            get => selectedHistoryItem;
            set
            {
                selectedHistoryItem = value;
                OnPropertyChanged();
            }
        }

        public ICommand UseSelectedCommand { get; }
        public ICommand DeleteSelectedCommand { get; }
        public ICommand ClearHistoryCommand { get; }

        public HistoryViewModel(IHistoryProvider historyProvider, Window window)
        {
            this.historyProvider = historyProvider;
            this.window = window;

            UseSelectedCommand = new RelayCommand(ExecuteUseSelected, CanUseSelected);
            DeleteSelectedCommand = new RelayCommand(ExecuteDeleteSelected, CanUseSelected);
            ClearHistoryCommand = new RelayCommand(ExecuteClearHistory);
        }

        private bool CanUseSelected(object? parameter) => SelectedHistoryItem != null;

        private void ExecuteUseSelected(object? parameter)
        {
            if (SelectedHistoryItem != null)
            {
                // IHistoryProvider에서 특정 ViewModel 타입에 따라 처리
                switch (historyProvider)
                {
                    case AdvancedCalculatorViewModel calcVM:
                        calcVM.CurrentInput = SelectedHistoryItem.Expression;
                        calcVM.Display = SelectedHistoryItem.Expression;
                        break;
                    case BaseConverterViewModel baseVM:
                        // Base converter의 경우 결과값을 입력으로 설정
                        baseVM.InputValue = SelectedHistoryItem.Result.ToString();
                        break;
                    case BitOperationsViewModel bitVM:
                        // Bit operations의 경우 결과값을 operand A로 설정
                        bitVM.OperandAInput = SelectedHistoryItem.Result.ToString();
                        break;
                }
            }
        }

        private void ExecuteDeleteSelected(object? parameter)
        {
            if (SelectedHistoryItem != null)
            {
                historyProvider.RemoveHistoryItem(SelectedHistoryItem);
            }
        }

        private void ExecuteClearHistory(object? parameter)
        {
            var result = MessageBox.Show(
                "Clear all calculation history?",
                "Confirm",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                historyProvider.ClearHistory();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}