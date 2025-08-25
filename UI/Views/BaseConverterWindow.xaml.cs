
// BaseConverterWindow.xaml.cs
using System.Windows;

namespace Calculator.UI.Views
{
    public partial class BaseConverterWindow : Window
    {
        public BaseConverterWindow()
        {
            InitializeComponent();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}