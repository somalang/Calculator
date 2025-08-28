using Calculator.UI.ViewModels;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace Calculator.UI.Views
{
    public partial class BitOperationsWindow : Window
    {
        private BitOperationsViewModel? ViewModel => DataContext as BitOperationsViewModel;

        public BitOperationsWindow()
        {
            InitializeComponent();
        }

        private void OperandA_GotFocus(object sender, RoutedEventArgs e)
        {
            if (DataContext is BitOperationsViewModel vm)
            {
                vm.HandleOperandAGotFocus();
                Debug.WriteLine($"OperandA got focus, IsOperandAFocused={vm.IsOperandAFocused}");
            }
        }

        private void OperandA_LostFocus(object sender, RoutedEventArgs e)
        {
            if (DataContext is BitOperationsViewModel vm)
            {
                vm.HandleOperandALostFocus();
                Debug.WriteLine($"OperandA lost focus, IsOperandAFocused={vm.IsOperandAFocused}");
            }
        }

        private void OperandB_GotFocus(object sender, RoutedEventArgs e)
        {
            if (DataContext is BitOperationsViewModel vm)
            {
                vm.HandleOperandBGotFocus();
                Debug.WriteLine($"OperandB got focus, IsOperandBFocused={vm.IsOperandBFocused}");
            }
        }

        private void OperandB_LostFocus(object sender, RoutedEventArgs e)
        {
            if (DataContext is BitOperationsViewModel vm)
            {
                vm.HandleOperandBLostFocus();
                Debug.WriteLine($"OperandB lost focus, IsOperandBFocused={vm.IsOperandBFocused}");
            }
        }

    }

}
