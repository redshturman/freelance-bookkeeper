using FreelanceBookkeeper.Data;
using FreelanceBookkeeper.Models;
using FreelanceBookkeeper.ViewModels;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FreelanceBookkeeper.Views
{
    /// <summary>
    /// Interaction logic for CustomerTransactionView.xaml
    /// </summary>
    public partial class CustomerTransactionView : Window
    {
        private readonly CustomerTransactionViewModel vm;

        public CustomerTransactionView()
        {
            InitializeComponent();
            vm = new CustomerTransactionViewModel();
            this.DataContext = vm;
        }

        private void MonthGroupComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataContext is not CustomerTransactionViewModel vm)
                return;

            var selected = (sender as ComboBox)?.SelectedItem as MonthGroup;

            int? group = selected?.Group;
            int? year = YearComboBox.SelectedItem as int?;

            vm.RefreshFilteredCustomerTransactions(year, group);
        }

        private void YearComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataContext is not CustomerTransactionViewModel vm)
                return;

            int? year = ((ComboBox)sender).SelectedItem as int?;
            int? group = (MonthGroupComboBox.SelectedItem as MonthGroup)?.Group;

            vm.RefreshFilteredCustomerTransactions(year, group);
        }
        
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is CustomerTransaction customerTransaction)
            {
                using var db = new AppDbContext();
                vm.DeleteCustomerTransaction(customerTransaction);
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
                Title = "Exportar Clients a Excel",
                FileName = $"Clients_{DateTime.Now:yyyy-MM-dd}.xlsx"
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
}
