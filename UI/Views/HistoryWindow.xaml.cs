using Calculator.Core.Services;
using Calculator.UI.ViewModels;
using System.Windows;

namespace Calculator.UI.Views
{
    public partial class HistoryWindow : Window
    {
        public HistoryWindow()
        {
            InitializeComponent();
            // DataContext는 외부에서 설정됩니다.
        }

        public HistoryWindow(IHistoryProvider provider)
        {
            InitializeComponent();
            // HistoryViewModel을 생성하여 DataContext로 설정
            DataContext = new HistoryViewModel(provider);
        }
    }
}