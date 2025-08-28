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

    }
}
