using System.Windows;
using Calculator.UI.ViewModels;

namespace Calculator.UI.Views
{
    public partial class HistoryWindow : Window
    {
        private readonly IHistoryProvider historyProvider;

        public HistoryWindow(IHistoryProvider provider)
        {
            InitializeComponent();
            historyProvider = provider;
            DataContext = historyProvider.HistoryItems;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}