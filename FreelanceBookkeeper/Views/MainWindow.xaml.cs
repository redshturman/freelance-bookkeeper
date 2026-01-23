using FreelanceBookkeeper.ViewModels;
using FreelanceBookkeeper.Views;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FreelanceBookkeeper.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            SetWindowSize();
        }

        private void OpenExpenseView_Click(object sender, RoutedEventArgs e)
        {
            var window = new ExpenseView();
            window.Show();
        }

        private void OpenCustomerTransactionView_Click(object sender, RoutedEventArgs e)
        {
            var window = new CustomerTransactionView();
            window.Show();
        }

        private void OpenSettings_Click(object sender, RoutedEventArgs e)
        {
            var window = new SettingsView();
            window.ShowDialog();
        }

        private void SetWindowSize()
        {
            var screen = System.Windows.SystemParameters.WorkArea;
            this.Width = screen.Width * 0.3;
            this.Height = screen.Height * 0.4;
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }
    }
}