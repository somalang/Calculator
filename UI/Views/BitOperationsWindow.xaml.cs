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

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void OperandA_GotFocus(object sender, RoutedEventArgs e)
        {
            if (DataContext is BitOperationsViewModel vm)
                vm.HandleOperandAGotFocus();
        }

        private void OperandA_LostFocus(object sender, RoutedEventArgs e)
        {
            if (DataContext is BitOperationsViewModel vm)
                vm.HandleOperandALostFocus();
        }

        private void OperandB_GotFocus(object sender, RoutedEventArgs e)
        {
            if (DataContext is BitOperationsViewModel vm)
                vm.HandleOperandBGotFocus();
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