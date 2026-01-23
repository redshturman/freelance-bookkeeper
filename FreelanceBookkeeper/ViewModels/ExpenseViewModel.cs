using FreelanceBookkeeper.Configuration;
using FreelanceBookkeeper.Data;
using FreelanceBookkeeper.Models;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace FreelanceBookkeeper.ViewModels;

public class ExpenseViewModel
{
    public ObservableCollection<Expense> Expenses { get; set; } = new();
    public Config config = Config.Load();
    private readonly List<int> deletedExpenseIds = new();


    public ExpenseViewModel()
    {
        config = Config.Load();
        LoadExpenses();
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
}
