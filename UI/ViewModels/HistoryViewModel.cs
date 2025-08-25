using Calculator.Core.Models;
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

            RefreshHistory(); // 초기 로드

            UseSelectedCommand = new RelayCommand(ExecuteUseSelected, CanUseSelected);
            ClearHistoryCommand = new RelayCommand(ExecuteClearHistory);
        }

        // 외부에서 호출할 수 있는 히스토리 새로고침 메서드
        public void RefreshHistory()
        {
            HistoryItems.Clear();

            // CalculatorViewModel에 Calculator 속성이 있는지 확인하고, 없으면 대안 방법 사용
            var historyItems = GetHistoryItems();

            foreach (var item in historyItems)
            {
                HistoryItems.Add(item);
            }
        }

        private System.Collections.Generic.IEnumerable<HistoryItem> GetHistoryItems()
        {
            // Calculator 속성에 접근할 수 있는지 확인
            var calculatorProperty = calculatorViewModel.GetType().GetProperty("Calculator");
            if (calculatorProperty != null)
            {
                var calculator = calculatorProperty.GetValue(calculatorViewModel);
                var historyProperty = calculator?.GetType().GetProperty("History");
                if (historyProperty != null)
                {
                    var history = historyProperty.GetValue(calculator);
                    var itemsProperty = history?.GetType().GetProperty("Items");
                    if (itemsProperty != null)
                    {
                        var items = itemsProperty.GetValue(history) as System.Collections.Generic.IEnumerable<HistoryItem>;
                        return items?.Reverse().Take(50) ?? new System.Collections.Generic.List<HistoryItem>();
                    }
                }
            }

            // 대안: CalculatorViewModel에서 히스토리를 직접 가져오는 메서드가 있는지 확인
            var getHistoryMethod = calculatorViewModel.GetType().GetMethod("GetHistory");
            if (getHistoryMethod != null)
            {
                var result = getHistoryMethod.Invoke(calculatorViewModel, null);
                if (result is System.Collections.Generic.IEnumerable<HistoryItem> historyItems)
                {
                    return historyItems.Reverse().Take(50);
                }
            }

            return new System.Collections.Generic.List<HistoryItem>();
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

                // 결과 표시 상태 해제 (새로운 입력 시작)
                // SetResultDisplayed 메서드가 있는지 확인하고 호출
                var setResultDisplayedMethod = calculatorViewModel.GetType().GetMethod("SetResultDisplayed");
                if (setResultDisplayedMethod != null)
                {
                    setResultDisplayedMethod.Invoke(calculatorViewModel, new object[] { false });
                }
                else
                {
                    // 대안: IsResultDisplayed 속성이 있다면 직접 설정
                    var isResultDisplayedProperty = calculatorViewModel.GetType().GetProperty("IsResultDisplayed");
                    if (isResultDisplayedProperty != null && isResultDisplayedProperty.CanWrite)
                    {
                        isResultDisplayedProperty.SetValue(calculatorViewModel, false);
                    }
                }

                // 창 닫기
                window?.Close();
            }
        }

        private void ExecuteClearHistory(object parameter)
        {
            var result = MessageBox.Show(
                "Clear all calculation history?",
                "Confirm",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                // ClearHistoryCommand가 있는지 확인하고 실행
                var clearHistoryCommand = calculatorViewModel.ClearHistoryCommand;
                if (clearHistoryCommand != null && clearHistoryCommand.CanExecute(null))
                {
                    clearHistoryCommand.Execute(null);
                }
                else
                {
                    // 대안: ClearHistory 메서드 직접 호출
                    var clearHistoryMethod = calculatorViewModel.GetType().GetMethod("ClearHistory");
                    if (clearHistoryMethod != null)
                    {
                        clearHistoryMethod.Invoke(calculatorViewModel, null);
                    }
                }

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