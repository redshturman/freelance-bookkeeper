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
                    CalculateTaxAmounts();
                    OnPropertyChanged(nameof(TotalAmount));
                    OnPropertyChanged(nameof(BaseAmount));
                    OnPropertyChanged(nameof(TaxAmount));
                }
            }
        }

        /// <summary>
        /// Gets or sets the date when the expense was incurred.
        /// </summary>
        public DateOnly ExpenseDate { get; set; }

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

        private void CalculateTaxAmounts()
        {
            // BaseAmount and TaxAmount are calculated properties, so no need to set them
        }
    }
}
