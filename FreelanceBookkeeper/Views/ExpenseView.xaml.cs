using FreelanceBookkeeper.Data;
using FreelanceBookkeeper.Models;
using FreelanceBookkeeper.ViewModels;
using Microsoft.Win32;
using System;
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

    private void ExportButton_Click(object sender, RoutedEventArgs e)
    {
        var saveFileDialog = new SaveFileDialog
        {
            Filter = "Excel Files|*.xlsx",
            Title = "Exportar Despeses a Excel",
            FileName = $"Despeses_{DateTime.Now:yyyy-MM-dd}.xlsx"
        };

        if (saveFileDialog.ShowDialog() == true)
        {
            try
            {
                vm.ExportToExcel(saveFileDialog.FileName);
                MessageBox.Show("Dades exportades correctament!", "Èxit", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al exportar: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    private async void SendEmailButton_Click(object sender, RoutedEventArgs e)
    {
        var emailDialog = new EmailSendView(vm.Years, vm.MonthGroups);
        emailDialog.ShowDialog();

        if (emailDialog.WasSent)
        {
            try
            {
                await vm.SendEmailWithExcel(
                    emailDialog.FinalRecipientEmail,
                    emailDialog.SelectedYear,
                    emailDialog.SelectedMonthGroup);

                MessageBox.Show("Correu enviat correctament!", "Èxit", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error a l'enviar el correu: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}