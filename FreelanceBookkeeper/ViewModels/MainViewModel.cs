using FreelanceBookkeeper.Data;
using FreelanceBookkeeper.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Media.Animation;

namespace FreelanceBookkeeper.ViewModels
{
    /// <summary>
    /// Represents the main view model for the application's user interface.
    /// </summary>
    public class MainViewModel
    {
        public ObservableCollection<Expense> Expenses { get; set; } = new ObservableCollection<Expense>();

        public MainViewModel()
        {
            
        }
    }
}
