using System.Collections.ObjectModel;
using FreelanceBookkeeper.Models;
using FreelanceBookkeeper.Data;

namespace FreelanceBookkeeper.ViewModels;

public class ExpenseViewModel
{
    public ObservableCollection<Expense> Expenses { get; set; } = new();

    public ExpenseViewModel()
    {
        LoadExpenses();
    }

    private void LoadExpenses()
    {
        using var db = new AppDbContext();
        var list = db.Expenses.OrderByDescending(e => e.ExpenseDate).ToList();
        Expenses.Clear();
        foreach (var e in list)
            Expenses.Add(e);
    }
}
