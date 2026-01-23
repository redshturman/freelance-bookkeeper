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

        InitializeYearCombo();
        InitializeMonthGroupCombo();
    }

    private void MonthGroupComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (MonthGroupComboBox.SelectedItem is ComboBoxItem selectedItem &&
            int.TryParse(selectedItem.Tag?.ToString(), out int group))
        {
            int? year = null;
            if (YearComboBox.SelectedItem is ComboBoxItem yearItem && int.TryParse(yearItem.Content.ToString(), out int y))
                year = y;

            vm.RefreshFilteredExpenses(year, group);
        }
    }

    private void YearComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (YearComboBox.SelectedItem is ComboBoxItem selectedItem &&
            int.TryParse(selectedItem.Content.ToString(), out int year))
        {
            int? monthGroup = null;
            if (MonthGroupComboBox.SelectedItem is ComboBoxItem groupItem && int.TryParse(groupItem.Tag?.ToString(), out int g))
                monthGroup = g;

            vm.RefreshFilteredExpenses(year, monthGroup);
        }
    }

    private void InitializeYearCombo()
    {
        YearComboBox.Items.Clear();
        foreach (var y in vm.AllYears())
        {
            YearComboBox.Items.Add(new ComboBoxItem { Content = y });
        }
    }

    private void InitializeMonthGroupCombo()
    {
        MonthGroupComboBox.Items.Clear();
        int monthsToShow = vm.config.MonthsToShow;
        int groups = 12 / monthsToShow;
        for (int i = 1; i <= groups; i++)
        {
            MonthGroupComboBox.Items.Add(new ComboBoxItem { Content = $"Meses {((i - 1) * monthsToShow + 1)}-{i * monthsToShow}", Tag = i });
        }
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