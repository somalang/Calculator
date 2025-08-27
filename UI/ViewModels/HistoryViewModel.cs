using Calculator.Core.Models;
using Calculator.Core.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Linq;

namespace Calculator.UI.ViewModels
{
    public class HistoryViewModel : INotifyPropertyChanged
    {
        private readonly IHistoryProvider historyProvider;
        private HistoryItem? selectedHistoryItem;
        private readonly Action<string>? setCalculatorInput; // 추가: 선택한 식 전달용

        public ObservableCollection<HistoryItem> HistoryItems => historyProvider.Items;

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
        public ICommand CloseCommand { get; }

        // 생성자에서 Action 전달
        public HistoryViewModel(IHistoryProvider historyProvider, Action<string>? setCalculatorInput = null)
        {
            this.historyProvider = historyProvider;
            this.setCalculatorInput = setCalculatorInput;

            UseSelectedCommand = new RelayCommand(ExecuteUseSelected, CanUseSelected);
            DeleteSelectedCommand = new RelayCommand(ExecuteDeleteSelected, CanUseSelected);
            ClearHistoryCommand = new RelayCommand(ExecuteClearHistory);
            CloseCommand = new RelayCommand(ExecuteClose);
        }

        private bool CanUseSelected(object? parameter) => SelectedHistoryItem != null;

        private void ExecuteUseSelected(object? parameter)
        {
            if (SelectedHistoryItem != null && setCalculatorInput != null)
            {
                setCalculatorInput(SelectedHistoryItem.Expression);
            }
        }


        private void ExecuteDeleteSelected(object? parameter)
        {
            if (SelectedHistoryItem != null)
            {
                historyProvider.Remove(SelectedHistoryItem);
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
                historyProvider.Clear();
            }
        }

        private void ExecuteClose(object? parameter)
        {
            var window = Application.Current.Windows
                .OfType<Window>()
                .FirstOrDefault(w => w.DataContext == this);
            window?.Close();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
