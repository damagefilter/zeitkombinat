using System.Collections.Generic;

namespace Hourglass.Models;

public class Project {
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string InvoiceMarker { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<Story> Stories { get; set; } = new();
    public List<Invoice> Invoices { get; set; } = new();
}
