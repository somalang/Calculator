using System.Windows;

namespace Calculator.UI
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var mainWindow = new Views.CalculatorView();
            mainWindow.Show();
        }
    }
}