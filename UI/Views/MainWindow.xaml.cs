// MenuWindow.xaml.cs
using Calculator.UI.ViewModels;
using System.Windows;

namespace Calculator.UI.Views
{
    public partial class MenuWindow : Window
    {
        public MenuWindow()
        {
            InitializeComponent();
            DataContext = new MenuViewModel(this);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}