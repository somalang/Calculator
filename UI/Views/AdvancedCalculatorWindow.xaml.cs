// AdvancedCalculatorWindow.xaml.cs
using Calculator.UI.ViewModels;
using System.Windows;

namespace Calculator.UI.Views
{
    public partial class AdvancedCalculatorWindow : Window
    {
        public AdvancedCalculatorWindow()
        {
            InitializeComponent();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}



