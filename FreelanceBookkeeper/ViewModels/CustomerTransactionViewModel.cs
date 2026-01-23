using ClosedXML.Excel;
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
        public ObservableCollection<int> Years { get; } = new();

        public Config config = Config.Load();
        private List<CustomerTransaction> allTransactions = new();
        public List<MonthGroup> MonthGroups
        {
            get
            {
                int months = config.MonthsToShow;

                if (months <= 0 || months > 12)
                    months = 3;

                int groups = 12 / months;

                var result = new List<MonthGroup>();

                for (int i = 1; i <= groups; i++)
                {
                    int start = (i - 1) * months + 1;
                    int end = start + months - 1;

                    result.Add(new MonthGroup
                    {
                        Group = i,
                        Display = $"Mesos {start}-{end}"
                    });
                }

                return result;
            }
        }

        public CustomerTransactionViewModel()
        {
            config = Config.Load();
            LoadInvoices();
            LoadYears();
        }

        private void LoadInvoices()
        {
            using var db = new AppDbContext();
            var list = db.CustomerTransactions.OrderByDescending(e => e.InvoiceDate).ToList();
            CustomerTransactions.Clear();
            foreach (var e in list)
               CustomerTransactions.Add(e);
        }

        private void LoadYears()
        {
            using var db = new AppDbContext();

            Years.Clear();

            foreach (var year in db.CustomerTransactions
                                    .Select(e => e.InvoiceDate.Year)
                                    .Distinct()
                                    .OrderByDescending(y => y))
            {
                Years.Add(year);
            }
        }

        public void RefreshFilteredCustomerTransactions(int? year = null, int? monthGroup = null)
        {
            using var db = new AppDbContext();
            var query = db.CustomerTransactions.AsQueryable();

            if (year.HasValue)
                query = query.Where(e => e.InvoiceDate.Year == year.Value);

            if (monthGroup.HasValue)
            {
                int monthsToShow = config.MonthsToShow;
                int startMonth = 1 + (monthGroup.Value - 1) * monthsToShow;
                int endMonth = startMonth + monthsToShow - 1;
                query = query.Where(e => e.InvoiceDate.Month >= startMonth && e.InvoiceDate.Month <= endMonth);
            }

            var list = query.OrderByDescending(e => e.InvoiceDate).ToList();

            CustomerTransactions.Clear();
            foreach (var e in list)
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

        public void FilterByYear(int? year)
        {
            using var db = new AppDbContext();

            var query = db.CustomerTransactions.AsQueryable();

            if (year.HasValue)
                query = query.Where(e => e.InvoiceDate.Year == year.Value);

            var list = query
                .OrderByDescending(e => e.InvoiceDate)
                .ToList();

            CustomerTransactions.Clear();
            foreach (var e in list)
                CustomerTransactions.Add(e);
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
                        entity.TaxPercentage = cust.TaxPercentage;
                    }
                }
            }

            db.SaveChanges();
        }

        public void ExportToExcel(string filePath)
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Clients");

            // Headers (same order as in DataGrid)
            worksheet.Cell(1, 1).Value = "Nº Factura";
            worksheet.Cell(1, 2).Value = "Nom";
            worksheet.Cell(1, 3).Value = "Adreça";
            worksheet.Cell(1, 4).Value = "Telèfon";
            worksheet.Cell(1, 5).Value = "DNI/NIE";
            worksheet.Cell(1, 6).Value = "Data";
            worksheet.Cell(1, 7).Value = "Total";
            worksheet.Cell(1, 8).Value = "Base Imponible";
            worksheet.Cell(1, 9).Value = "Import IVA (21%)";

            // Style headers
            var headerRange = worksheet.Range(1, 1, 1, 9);
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
            headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            // Data rows
            int row = 2;
            foreach (var transaction in CustomerTransactions)
            {
                worksheet.Cell(row, 1).Value = transaction.InvoiceNumber;
                worksheet.Cell(row, 2).Value = transaction.Name;
                worksheet.Cell(row, 3).Value = transaction.Address;
                worksheet.Cell(row, 4).Value = transaction.PhoneNumber;
                worksheet.Cell(row, 5).Value = transaction.Identification;
                worksheet.Cell(row, 6).Value = transaction.InvoiceDate.ToString("dd/MM/yyyy");
                worksheet.Cell(row, 7).Value = transaction.TotalAmount;
                worksheet.Cell(row, 8).Value = transaction.BaseAmount;
                worksheet.Cell(row, 9).Value = transaction.TaxAmount;

                // Format currency columns
                worksheet.Cell(row, 7).Style.NumberFormat.Format = "#,##0.00";
                worksheet.Cell(row, 8).Style.NumberFormat.Format = "#,##0.00";
                worksheet.Cell(row, 9).Style.NumberFormat.Format = "#,##0.00";

                row++;
            }

            // Auto-fit columns
            worksheet.Columns().AdjustToContents();

            workbook.SaveAs(filePath);
        }
    }
}
