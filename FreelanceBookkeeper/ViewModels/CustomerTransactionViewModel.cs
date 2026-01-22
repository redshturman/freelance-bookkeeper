using FreelanceBookkeeper.Data;
using FreelanceBookkeeper.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace FreelanceBookkeeper.ViewModels
{
    class CustomerTransactionViewModel
    {
        public ObservableCollection<CustomerTransaction> CustomerTransactions { get; set; } = new();

        public CustomerTransactionViewModel()
        {
            LoadInvoices();
        }

        private void LoadInvoices()
        {
            using var db = new AppDbContext();
            var list = db.CustomerTransactions.OrderByDescending(e => e.InvoiceDate).ToList();
            CustomerTransactions.Clear();
            foreach (var e in list)
               CustomerTransactions.Add(e);
        }
    }
}
