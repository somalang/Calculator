using System.Windows;

namespace Calculator.UI.Views
{
    public partial class CalculatorView : Window
    {
        public CalculatorView()
        {
            InitializeComponent();
        }
        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            MenuWindow menuWindow = new MenuWindow();
            menuWindow.Show();
            this.Close(); // 현재 창 닫기
        }

        private void TextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {

        }
    }

}
