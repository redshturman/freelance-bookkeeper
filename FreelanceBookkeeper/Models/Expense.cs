using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace FreelanceBookkeeper.Models
{
    /// <summary>
    /// Represents an expense for a freelance.
    /// </summary>
    public class Expense : INotifyPropertyChanged
    {
        private decimal _totalAmount;
        private DateOnly _expenseDate;

        /// <summary>
        /// Gets or sets the unique identifier for the expense.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the supplier name associated with the expense.
        /// </summary>
        public string SupplierName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the supplier identification number associated with the expense.
        /// </summary>
        public string SupplierId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the description of the expense.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the total amount of the expense. Tax included.
        /// </summary>
        public decimal TotalAmount
        {
            get => _totalAmount;
            set
            {
                if (_totalAmount != value)
                {
                    _totalAmount = value;
                    OnPropertyChanged(nameof(TotalAmount));
                    OnPropertyChanged(nameof(BaseAmount));
                    OnPropertyChanged(nameof(TaxAmount));
                }
            }
        }

        /// <summary>
        /// Gets or sets the date when the expense was incurred.
        /// </summary>
        public DateOnly ExpenseDate
        {
            get => _expenseDate;
            set
            {
                if (_expenseDate != value)
                {
                    _expenseDate = value;
                    OnPropertyChanged(nameof(ExpenseDate));
                    OnPropertyChanged(nameof(ExpenseDateAsDateTime));
                }
            }
        }

        /// <summary>
        /// Helper property for WPF DatePicker binding.
        /// </summary>
        [NotMapped]
        public DateTime? ExpenseDateAsDateTime
        {
            get => ExpenseDate == default ? null : ExpenseDate.ToDateTime(TimeOnly.MinValue);
            set
            {
                if (value.HasValue)
                {
                    ExpenseDate = DateOnly.FromDateTime(value.Value);
                }
                else
                {
                    ExpenseDate = DateOnly.FromDateTime(DateTime.Today);
                }
            }
        }

        /// <summary>
        /// Gets the base amount without tax (IVA). Calculated automatically.
        /// </summary>
        [NotMapped]
        public decimal BaseAmount => Math.Round(_totalAmount / (1 + TaxPercentage / 100), 2);

        /// <summary>
        /// Gets the IVA (tax) amount. Calculated automatically.
        /// </summary>
        [NotMapped]
        public decimal TaxAmount => Math.Round(_totalAmount - BaseAmount, 2);

        /// <summary>
        /// Gets or sets the IVA (tax) percentage. Default is 21% (standard Spanish IVA).
        /// </summary>
        public decimal TaxPercentage { get; set; } = 21m;

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
