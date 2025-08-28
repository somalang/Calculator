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
        public ICommand? CloseCommand { get; }
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
            CloseCommand = new RelayCommand(ExecuteClose);

        }

        private void ExecuteStandardCalculator(object? parameter)
        {
            // 메인 창이 이미 존재하면 메뉴만 닫음
            var mainWindow = Application.Current.MainWindow;
            if (mainWindow == null || mainWindow is not CalculatorView)
            {
                var standardWindow = new CalculatorView();
                //standardWindow.Owner = mainWindow ?? owner;
                standardWindow.Show();
                Application.Current.MainWindow = standardWindow;
            }

            owner.Close();
        }

        private void ExecuteAdvancedCalculator(object? parameter)
        {
            var advancedWindow = new AdvancedCalculatorWindow();
            //advancedWindow.Owner = Application.Current.MainWindow ?? owner;
            advancedWindow.Show();
            owner.Close();
        }

        private void ExecuteBaseConverter(object? parameter)
        {
            var baseConverterWindow = new BaseConverterWindow();
            //baseConverterWindow.Owner = Application.Current.MainWindow ?? owner;
            baseConverterWindow.Show();
            owner.Close();
        }

        private void ExecuteBitOperations(object? parameter)
        {
            var bitOperationsWindow = new BitOperationsWindow();
            //bitOperationsWindow.Owner = Application.Current.MainWindow ?? owner;
            bitOperationsWindow.Show();
            owner.Close();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private void ExecuteClose(object? parameter)
        {
            var window = Application.Current.Windows
                .OfType<Window>()
                .FirstOrDefault(w => w.DataContext == this);
            window?.Close();
        }
    }
}
