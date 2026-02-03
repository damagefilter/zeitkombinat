namespace Zeitkombinat.Models;

public class InvoiceTask {
    public int Id { get; set; }
    public int InvoiceId { get; set; }
    public Invoice Invoice { get; set; } = null!;
    public int TaskItemId { get; set; }
    public TaskItem TaskItem { get; set; } = null!;
    public decimal? HourlyRateOverride { get; set; }
}
