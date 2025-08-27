// Calculator.UI/ViewModels/HistoryViewModel.cs
using Calculator.Core.Models;
using Calculator.Core.Services; // 인터페이스 가져오기
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
        private HistoryItem? selectedHistoryItem;

        // 인터페이스가 노출하는 Items 사용
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

        public HistoryViewModel(IHistoryProvider historyProvider)
        {
            this.historyProvider = historyProvider;

            UseSelectedCommand = new RelayCommand(ExecuteUseSelected, CanUseSelected);
            DeleteSelectedCommand = new RelayCommand(ExecuteDeleteSelected, CanUseSelected);
            ClearHistoryCommand = new RelayCommand(ExecuteClearHistory);
        }

        private bool CanUseSelected(object? parameter) => SelectedHistoryItem != null;

        private void ExecuteUseSelected(object? parameter)
        {
            if (SelectedHistoryItem == null) return;

            // UI와 다른 ViewModel 연동 로직
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

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
