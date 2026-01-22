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

    }
}