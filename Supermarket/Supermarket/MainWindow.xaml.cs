using Supermarket.Views;
using System.Windows;

namespace Supermarket
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }


        public void OnClick_Login(object sender, RoutedEventArgs e)
        {
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            
        }

        public void OnClick_Exit(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
