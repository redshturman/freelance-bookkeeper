using FreelanceBookkeeper.Data;
using FreelanceBookkeeper.Models;
using FreelanceBookkeeper.ViewModels;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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

    private void MonthGroupComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (DataContext is not ExpenseViewModel vm)
            return;

        var selected = (sender as ComboBox)?.SelectedItem as MonthGroup;

        int? group = selected?.Group;
        int? year = YearComboBox.SelectedItem as int?;

        vm.RefreshFilteredExpenses(year, group);
    }

    private void YearComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (DataContext is not ExpenseViewModel vm)
            return;

        int? year = ((ComboBox)sender).SelectedItem as int?;
        int? group = (MonthGroupComboBox.SelectedItem as MonthGroup)?.Group;

        vm.RefreshFilteredExpenses(year, group);
    }

    private void DeleteButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is Expense expense)
        {
            using var db = new AppDbContext();
            vm.DeleteExpense(expense);
        }
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        vm.SaveAll();
        MessageBox.Show("Cambios guardados en la base de datos");
    }
}