using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace FreelanceBookkeeper.Models
{
    /// <summary>
    /// Represents an expense for a freelance.
    /// </summary>
    public class Expense
    {
        /// <summary>
        /// Gets or sets the unique identifier for the expense.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the he supplier name associated with the expense.
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
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// Gets or sets the date when the expense was incurred.
        /// </summary>
        public DateOnly ExpenseDate { get; set; }
    }
}
