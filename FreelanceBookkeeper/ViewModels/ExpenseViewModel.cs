using ClosedXML.Excel;
using FreelanceBookkeeper.Configuration;
using FreelanceBookkeeper.Data;
using FreelanceBookkeeper.Models;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

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

        // Headers (same order as in DataGrid)
        worksheet.Cell(1, 1).Value = "Nom";
        worksheet.Cell(1, 2).Value = "Concepte";
        worksheet.Cell(1, 3).Value = "DNI/NIE";
        worksheet.Cell(1, 4).Value = "Data";
        worksheet.Cell(1, 5).Value = "Total";
        worksheet.Cell(1, 6).Value = "Base Imponible";
        worksheet.Cell(1, 7).Value = "Import IVA (21%)";

        // Style headers
        var headerRange = worksheet.Range(1, 1, 1, 7);
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
        headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        // Data rows
        int row = 2;
        foreach (var expense in Expenses)
        {
            worksheet.Cell(row, 1).Value = expense.SupplierName;
            worksheet.Cell(row, 2).Value = expense.Description;
            worksheet.Cell(row, 3).Value = expense.SupplierId;
            worksheet.Cell(row, 4).Value = expense.ExpenseDate.ToString("dd/MM/yyyy");
            worksheet.Cell(row, 5).Value = expense.TotalAmount;
            worksheet.Cell(row, 6).Value = expense.BaseAmount;
            worksheet.Cell(row, 7).Value = expense.TaxAmount;

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
}
