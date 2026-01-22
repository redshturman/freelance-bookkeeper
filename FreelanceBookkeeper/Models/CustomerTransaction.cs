using System;
using System.Collections.Generic;
using System.Text;

namespace FreelanceBookkeeper.Models
{
    /// <summary>
    /// Represents the customer transaction.
    /// </summary>
    public class CustomerTransaction
    {
        /// <summary>
        /// Gets or sets the unique identifier for the customer transaction.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the customer.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the identification number of the customer.
        /// </summary>
        public string? Identification { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the address of the customer.
        /// </summary>
        public string Address { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the phone number of the customer.
        /// </summary>
        public string PhoneNumber { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the unique number assigned to the invoice.
        /// </summary>
        public int InvoiceNumber { get; set; }

        /// <summary>
        /// Gets or sets the total amount of the invoice. Tax included.
        /// </summary>
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// Gets or sets the date when the invoice was issued.
        /// </summary>
        public DateOnly InvoiceDate { get; set; }
    }
}
