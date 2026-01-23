using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace FreelanceBookkeeper.Models;

/// <summary>
/// Represents the customer transaction.
/// </summary>
public class CustomerTransaction : INotifyPropertyChanged
{
    private decimal _totalAmount;

    /// <summary>
    /// Gets or sets the unique identifier for the customer transaction.
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
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
    /// Gets or sets the date when the invoice was issued.
    /// </summary>
    public DateOnly InvoiceDate { get; set; }

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
