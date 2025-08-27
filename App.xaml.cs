using Calculator.Core.Services;
using System.Windows;

namespace Calculator
{
    public partial class App : Application
    {
        public static HistoryService? HistoryService { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // HistoryService 싱글톤 초기화
            HistoryService = new HistoryService();
        }
    }
}