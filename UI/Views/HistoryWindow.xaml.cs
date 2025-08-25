using System.Windows;
using Calculator.UI.ViewModels;

namespace Calculator.UI.Views
{
    public partial class HistoryWindow : Window
    {
        public HistoryWindow(CalculatorViewModel calculatorViewModel)
        {
            InitializeComponent();
            DataContext = new HistoryViewModel(calculatorViewModel, this);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}