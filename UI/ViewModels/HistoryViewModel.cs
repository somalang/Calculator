using Calculator.Core.Models;
using Calculator.Core.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace Calculator.UI.ViewModels
{
    public class HistoryViewModel : INotifyPropertyChanged
    {
        private readonly CalculatorViewModel calculatorViewModel;
        private readonly Window window;
        private HistoryItem? selectedHistoryItem;

        // CalculatorViewModel의 HistoryItems를 직접 참조
        public ObservableCollection<HistoryItem> HistoryItems => calculatorViewModel.HistoryItems;

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

        public HistoryViewModel(CalculatorViewModel calculatorViewModel, Window window)
        {
            this.calculatorViewModel = calculatorViewModel;
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
                calculatorViewModel.CurrentInput = SelectedHistoryItem!.Expression;
                calculatorViewModel.Display = SelectedHistoryItem!.Expression;
            }
        }

        private void ExecuteDeleteSelected(object? parameter)
        {
            if (SelectedHistoryItem != null)
            {
                calculatorViewModel.History.Remove(SelectedHistoryItem);
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
                calculatorViewModel.History.Clear();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}