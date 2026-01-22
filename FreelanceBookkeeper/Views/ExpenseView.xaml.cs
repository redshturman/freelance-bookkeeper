using FreelanceBookkeeper.ViewModels;
using System.Windows;

namespace FreelanceBookkeeper.Views;

public partial class ExpenseView : Window
{
    private readonly ExpenseViewModel vm;

    public ExpenseView()
    {
        InitializeComponent();
        vm = new ExpenseViewModel();
        this.DataContext = vm;
    }
}