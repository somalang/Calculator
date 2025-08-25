// BitOperationsWindow.xaml.cs
using System.Windows;

namespace Calculator.UI.Views
{
    public partial class BitOperationsWindow : Window
    {
        public BitOperationsWindow()
        {
            InitializeComponent();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
