using Calculator.Core.Services;
using System.Windows;

public partial class App : Application
{
    public static HistoryService? HistoryService { get; private set; }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        HistoryService = new HistoryService();
    }
}