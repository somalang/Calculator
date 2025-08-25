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
        private readonly CalculatorViewModel calculatorViewModel;
        private readonly Window window;
        private HistoryItem selectedHistoryItem;

        public ObservableCollection<HistoryItem> HistoryItems { get; }

        public HistoryItem SelectedHistoryItem
        {
            get => selectedHistoryItem;
            set
            {
                selectedHistoryItem = value;
                OnPropertyChanged();
            }
        }

        public ICommand UseSelectedCommand { get; }
        public ICommand ClearHistoryCommand { get; }

        public HistoryViewModel(CalculatorViewModel calculatorViewModel, Window window)
        {
            this.calculatorViewModel = calculatorViewModel;
            this.window = window;

            HistoryItems = new ObservableCollection<HistoryItem>();
            LoadHistory();

            UseSelectedCommand = new RelayCommand(ExecuteUseSelected, CanUseSelected);
            ClearHistoryCommand = new RelayCommand(ExecuteClearHistory);

            // 메인 계산기의 히스토리가 업데이트될 때마다 갱신
            calculatorViewModel.PropertyChanged += OnCalculatorViewModelPropertyChanged;
        }

        private void OnCalculatorViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(CalculatorViewModel.HistoryItems))
            {
                LoadHistory();
            }
        }

        private void LoadHistory()
        {
            HistoryItems.Clear();
            foreach (var item in calculatorViewModel.HistoryItems)
            {
                HistoryItems.Add(item);
            }
        }

        private bool CanUseSelected(object parameter)
        {
            return SelectedHistoryItem != null;
        }

        private void ExecuteUseSelected(object parameter)
        {
            if (SelectedHistoryItem != null)
            {
                // 선택된 계산식을 메인 계산기에 입력
                calculatorViewModel.CurrentInput = SelectedHistoryItem.Expression;
                calculatorViewModel.Display = SelectedHistoryItem.Expression;

                // 히스토리 창 닫기
                window.Close();
            }
        }

        private void ExecuteClearHistory(object parameter)
        {
            var result = MessageBox.Show(
                "모든 계산 히스토리를 삭제하시겠습니까?",
                "확인",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                calculatorViewModel.ClearHistoryCommand.Execute(null);
                HistoryItems.Clear();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}