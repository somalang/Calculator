using System.Windows;
using Calculator.Core.Services;

namespace Calculator.UI.Views
{
    public partial class HistoryWindow : Window
    {
        private readonly IHistoryProvider historyProvider;

        public HistoryWindow(IHistoryProvider provider)
        {
            InitializeComponent();
            historyProvider = provider;
            DataContext = historyProvider.Items;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}