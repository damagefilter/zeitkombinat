using System;
using System.Collections.Generic;

namespace Hourglass.Models;

public class Invoice {
    public int Id { get; set; }
    public string BillingId { get; set; } = string.Empty;
    public DateTime CreationDate { get; set; }
    public decimal HourlyRate { get; set; }
    public decimal TotalAmount { get; set; }
    public int ProjectId { get; set; }
    public Project Project { get; set; } = null!;
    public List<InvoiceTask> InvoiceTasks { get; set; } = new();
}
