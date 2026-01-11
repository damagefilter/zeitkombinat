using System;
using Hourglass.Models;

namespace Hourglass.ViewModels;

public class InvoiceViewModel {
    public Invoice Invoice { get; }

    public InvoiceViewModel(Invoice invoice) {
        Invoice = invoice;
    }

    public string BillingId => Invoice.BillingId;
    public string CreationDateText => $"Date: {Invoice.CreationDate:yyyy-MM-dd}";
    public string HourlyRateText => $"Hourly Rate: ${Invoice.HourlyRate:F2}";
    public string TotalAmountText => $"Total: ${Invoice.TotalAmount:F2}";
}
