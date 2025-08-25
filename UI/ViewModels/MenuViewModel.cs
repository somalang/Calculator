using Calculator.UI.Views;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace Calculator.UI.ViewModels
{
    public class MenuViewModel : INotifyPropertyChanged
    {
        private readonly Window owner;

        public ICommand StandardCalculatorCommand { get; }
        public ICommand AdvancedCalculatorCommand { get; }
        public ICommand BaseConverterCommand { get; }
        public ICommand BitOperationsCommand { get; }

        public MenuViewModel(Window owner)
        {
            this.owner = owner;

            StandardCalculatorCommand = new RelayCommand(ExecuteStandardCalculator);
            AdvancedCalculatorCommand = new RelayCommand(ExecuteAdvancedCalculator);
            BaseConverterCommand = new RelayCommand(ExecuteBaseConverter);
            BitOperationsCommand = new RelayCommand(ExecuteBitOperations);
        }

        private void ExecuteStandardCalculator(object parameter)
        {
            // 기본 계산기는 이미 메인 창에 있으므로 메뉴만 닫음
            ((Window)owner).Close();
        }

        private void ExecuteAdvancedCalculator(object parameter)
        {
            var advancedWindow = new AdvancedCalculatorWindow();
            advancedWindow.Owner = Application.Current.MainWindow;
            advancedWindow.Show();
            ((Window)owner).Close();
        }

        private void ExecuteBaseConverter(object parameter)
        {
            var baseConverterWindow = new BaseConverterWindow();
            baseConverterWindow.Owner = Application.Current.MainWindow;
            baseConverterWindow.Show();
            ((Window)owner).Close();
        }

        private void ExecuteBitOperations(object parameter)
        {
            var bitOperationsWindow = new BitOperationsWindow();
            bitOperationsWindow.Owner = Application.Current.MainWindow;
            bitOperationsWindow.Show();
            ((Window)owner).Close();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}