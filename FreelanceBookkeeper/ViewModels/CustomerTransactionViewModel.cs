using FreelanceBookkeeper.Configuration;
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
        public Config config = Config.Load();
        private List<CustomerTransaction> allTransactions = new();

        public CustomerTransactionViewModel()
        {
            config = Config.Load();
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

        public void RefreshFilteredExpenses(int? year = null, int? monthGroup = null)
        {
            var filtered = allTransactions.AsEnumerable();

            if (year.HasValue)
                filtered = filtered.Where(e => e.InvoiceDate.Year == year.Value);

            if (monthGroup.HasValue)
            {
                int monthsToShow = config.MonthsToShow;
                int startMonth = 1 + (monthGroup.Value - 1) * monthsToShow;
                int endMonth = startMonth + monthsToShow - 1;
                filtered = filtered.Where(e => e.InvoiceDate.Month >= startMonth && e.InvoiceDate.Month <= endMonth);
            }

            CustomerTransactions.Clear();
            foreach (var e in filtered.OrderByDescending(e => e.InvoiceDate))
                CustomerTransactions.Add(e);
        }

        public IEnumerable<int> AllYears()
        {
            return allTransactions
                .Select(e => e.InvoiceDate.Year)
                .Distinct()
                .OrderByDescending(y => y);
        }

        public void DeleteCustomerTransaction(CustomerTransaction customerTransaction)
        {
            using var db = new AppDbContext();

            if (customerTransaction.Id != 0)
            {
                var entity = db.CustomerTransactions.Find(customerTransaction.Id);
                if (entity != null)
                    db.CustomerTransactions.Remove(entity);

                db.SaveChanges();
            }

            CustomerTransactions.Remove(customerTransaction);
        }

        public void SaveAll()
        {
            using var db = new AppDbContext();

            foreach (var cust in CustomerTransactions)
            {
                if (cust.Id == 0)
                    db.CustomerTransactions.Add(cust);
                else
                {
                    var entity = db.CustomerTransactions.Find(cust.Id);
                    if (entity != null)
                    {
                        entity.Name = cust.Name;
                        entity.Identification = cust.Identification;
                        entity.Address = cust.Address;
                        entity.PhoneNumber = cust.PhoneNumber;
                        entity.InvoiceNumber = cust.InvoiceNumber;
                        entity.TotalAmount = cust.TotalAmount;
                        entity.InvoiceDate = cust.InvoiceDate;
                    }
                }
            }

            db.SaveChanges();
        }
    }
}
