using Calculator.UI.ViewModels;
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

        private void OperandATextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            ViewModel?.HandleOperandAGotFocus();

            // 텍스트 전체 선택 (사용자 편의성)
            if (sender is TextBox textBox)
            {
                textBox.SelectAll();
            }
        }

        private void OperandATextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            ViewModel?.HandleOperandALostFocus();
        }

        private void OperandBTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            ViewModel?.HandleOperandBGotFocus();

            // 텍스트 전체 선택 (사용자 편의성)
            if (sender is TextBox textBox)
            {
                textBox.SelectAll();
            }
        }

        private void OperandBTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            ViewModel?.HandleOperandBLostFocus();
        }
    }
}