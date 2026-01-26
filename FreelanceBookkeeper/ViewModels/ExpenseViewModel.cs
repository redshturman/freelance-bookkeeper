using ClosedXML.Excel;
using FreelanceBookkeeper.Configuration;
using FreelanceBookkeeper.Data;
using FreelanceBookkeeper.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;

namespace FreelanceBookkeeper.ViewModels;

public class ExpenseViewModel
{
    public ObservableCollection<Expense> Expenses { get; set; } = new();
    public ObservableCollection<int> Years { get; } = new();
    public Config config = Config.Load();
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

    public ExpenseViewModel()
    {
        config = Config.Load();
        LoadExpenses();
        LoadYears();
    }

    private void LoadExpenses()
    {
        using var db = new AppDbContext();
        var list = db.Expenses
                     .OrderByDescending(e => e.ExpenseDate)
                     .ToList();

        Expenses.Clear();
        foreach (var e in list)
            Expenses.Add(e);
    }

    private void LoadYears()
    {
        using var db = new AppDbContext();

        Years.Clear();

        foreach (var year in db.Expenses
                                .Select(e => e.ExpenseDate.Year)
                                .Distinct()
                                .OrderByDescending(y => y))
        {
            Years.Add(year);
        }
    }

    public void RefreshFilteredExpenses(int? year = null, int? monthGroup = null)
    {
        using var db = new AppDbContext();
        var query = db.Expenses.AsQueryable();

        if (year.HasValue)
            query = query.Where(e => e.ExpenseDate.Year == year.Value);

        if (monthGroup.HasValue)
        {
            int monthsToShow = config.MonthsToShow;
            int startMonth = 1 + (monthGroup.Value - 1) * monthsToShow;
            int endMonth = startMonth + monthsToShow - 1;

            query = query.Where(e =>
                e.ExpenseDate.Month >= startMonth &&
                e.ExpenseDate.Month <= endMonth);
        }

        var list = query.OrderByDescending(e => e.ExpenseDate).ToList();

        Expenses.Clear();
        foreach (var e in list)
            Expenses.Add(e);
    }

    public IEnumerable<int> AllYears()
    {
        using var db = new AppDbContext();

        return db.Expenses
                 .Select(e => e.ExpenseDate.Year)
                 .Distinct()
                 .OrderByDescending(y => y)
                 .ToList();
    }

    public void FilterByYear(int? year)
    {
        using var db = new AppDbContext();

        var query = db.Expenses.AsQueryable();

        if (year.HasValue)
            query = query.Where(e => e.ExpenseDate.Year == year.Value);

        var list = query
            .OrderByDescending(e => e.ExpenseDate)
            .ToList();

        Expenses.Clear();
        foreach (var e in list)
            Expenses.Add(e);
    }

    public void SaveAll()
    {
        using var db = new AppDbContext();

        foreach (var exp in Expenses)
        {
            if (exp.Id == 0)
                db.Expenses.Add(exp);
            else
            {
                var entity = db.Expenses.Find(exp.Id);
                if (entity != null)
                {
                    entity.SupplierName = exp.SupplierName;
                    entity.SupplierId = exp.SupplierId;
                    entity.Description = exp.Description;
                    entity.TotalAmount = exp.TotalAmount;
                    entity.ExpenseDate = exp.ExpenseDate;
                    entity.TaxPercentage = exp.TaxPercentage;
                }
            }
        }

        db.SaveChanges();
    }

    public void DeleteExpense(Expense expense)
    {
        using var db = new AppDbContext();

        if (expense.Id != 0)
        {
            var entity = db.Expenses.Find(expense.Id);
            if (entity != null)
                db.Expenses.Remove(entity);

            db.SaveChanges();
        }

        Expenses.Remove(expense);
    }

    public void ExportToExcel(string filePath)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Despeses");

        // Headers (matching DataGrid column order exactly)
        worksheet.Cell(1, 1).Value = "Data";
        worksheet.Cell(1, 2).Value = "Concepte";
        worksheet.Cell(1, 3).Value = "Nom Proveidor";
        worksheet.Cell(1, 4).Value = "DNI/NIE Proveidor";
        worksheet.Cell(1, 5).Value = "Base Imponible";
        worksheet.Cell(1, 6).Value = "Import IVA (21%)";
        worksheet.Cell(1, 7).Value = "Total";

        // Style headers
        var headerRange = worksheet.Range(1, 1, 1, 7);
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
        headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        // Data rows (same order as headers)
        int row = 2;
        foreach (var expense in Expenses)
        {
            worksheet.Cell(row, 1).Value = expense.ExpenseDate.ToString("dd/MM/yyyy");
            worksheet.Cell(row, 2).Value = expense.Description;
            worksheet.Cell(row, 3).Value = expense.SupplierName;
            worksheet.Cell(row, 4).Value = expense.SupplierId;
            worksheet.Cell(row, 5).Value = expense.BaseAmount;
            worksheet.Cell(row, 6).Value = expense.TaxAmount;
            worksheet.Cell(row, 7).Value = expense.TotalAmount;

            // Format currency columns
            worksheet.Cell(row, 5).Style.NumberFormat.Format = "#,##0.00";
            worksheet.Cell(row, 6).Style.NumberFormat.Format = "#,##0.00";
            worksheet.Cell(row, 7).Style.NumberFormat.Format = "#,##0.00";

            row++;
        }

        // Auto-fit columns
        worksheet.Columns().AdjustToContents();

        workbook.SaveAs(filePath);
    }

    public async Task SendEmailWithExcel(string recipientEmail, int? year = null, int? monthGroup = null)
    {
        // Load filtered data
        using var db = new AppDbContext();
        var query = db.Expenses.AsQueryable();

        if (year.HasValue)
            query = query.Where(e => e.ExpenseDate.Year == year.Value);

        if (monthGroup.HasValue)
        {
            int monthsToShow = config.MonthsToShow;
            int startMonth = 1 + (monthGroup.Value - 1) * monthsToShow;
            int endMonth = startMonth + monthsToShow - 1;

            query = query.Where(e =>
                e.ExpenseDate.Month >= startMonth &&
                e.ExpenseDate.Month <= endMonth);
        }

        var expenses = query.OrderByDescending(e => e.ExpenseDate).ToList();

        // Generate Excel in memory
        using var memoryStream = new MemoryStream();
        using (var workbook = new XLWorkbook())
        {
            var worksheet = workbook.Worksheets.Add("Despeses");

            // Headers (matching DataGrid column order exactly)
            worksheet.Cell(1, 1).Value = "Data";
            worksheet.Cell(1, 2).Value = "Concepte";
            worksheet.Cell(1, 3).Value = "Nom Proveidor";
            worksheet.Cell(1, 4).Value = "DNI/NIE Proveidor";
            worksheet.Cell(1, 5).Value = "Base Imponible";
            worksheet.Cell(1, 6).Value = "Import IVA (21%)";
            worksheet.Cell(1, 7).Value = "Total";

            var headerRange = worksheet.Range(1, 1, 1, 7);
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
            headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            // Data rows (same order as headers)
            int row = 2;
            foreach (var expense in expenses)
            {
                worksheet.Cell(row, 1).Value = expense.ExpenseDate.ToString("dd/MM/yyyy");
                worksheet.Cell(row, 2).Value = expense.Description;
                worksheet.Cell(row, 3).Value = expense.SupplierName;
                worksheet.Cell(row, 4).Value = expense.SupplierId;
                worksheet.Cell(row, 5).Value = expense.BaseAmount;
                worksheet.Cell(row, 6).Value = expense.TaxAmount;
                worksheet.Cell(row, 7).Value = expense.TotalAmount;

                worksheet.Cell(row, 5).Style.NumberFormat.Format = "#,##0.00";
                worksheet.Cell(row, 6).Style.NumberFormat.Format = "#,##0.00";
                worksheet.Cell(row, 7).Style.NumberFormat.Format = "#,##0.00";

                row++;
            }

            worksheet.Columns().AdjustToContents();
            workbook.SaveAs(memoryStream);
        }

        memoryStream.Position = 0;

        // Create email
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("Francesc Xavier Jové Pérez", config.SmtpUsername));
        message.To.Add(new MailboxAddress("", recipientEmail));
        message.Subject = $"Despeses Xavi Jové - {DateTime.Now:dd/MM/yyyy}";

        var builder = new BodyBuilder
        {
            TextBody = $"Hola,\n\nAdjunt trobaràs l'informe de despeses.\nAny: {year?.ToString() ?? "Tots"}\nTrimestre: {(monthGroup.HasValue ? $"Mesos {(monthGroup.Value - 1) * config.MonthsToShow + 1}-{monthGroup.Value * config.MonthsToShow}" : "Tots")}\n\n Gràcies,\n Francesc Xavier Jové Pérez."
        };

        builder.Attachments.Add($"Despeses_{DateTime.Now:yyyy-MM-dd}.xlsx", memoryStream.ToArray(), new ContentType("application", "vnd.openxmlformats-officedocument.spreadsheetml.sheet"));

        message.Body = builder.ToMessageBody();

        // Send email
        using var client = new SmtpClient();
        await client.ConnectAsync(config.SmtpServer, config.SmtpPort, config.SmtpUseSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None);
        await client.AuthenticateAsync(config.SmtpUsername, config.SmtpPassword);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
}
