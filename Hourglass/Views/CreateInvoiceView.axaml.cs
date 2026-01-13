using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Microsoft.EntityFrameworkCore;
using Hourglass.Models;
using Hourglass.ViewModels;

namespace Hourglass.Views;

public partial class CreateInvoiceView : HourglassControl {
    private readonly Project project;
    private List<InvoiceTaskItemViewModel> allTasks = new();

    public CreateInvoiceView() {
        InitializeComponent();
        project = new Project();
    }

    public CreateInvoiceView(Project project) {
        InitializeComponent();
        this.project = db.Projects
            .AsSplitQuery()
            .Include(p => p.Stories)
                .ThenInclude(s => s.Tasks)
            .ThenInclude(t => t.WorkSessions)
            .First(p => p.Id == project.Id);

        ProjectTitle.Text = $"Create Invoice for: {this.project.Name}";
        LoadTasks();
    }

    private void LoadTasks() {
        allTasks = project.Stories
            .SelectMany(s => s.Tasks)
            .Select(t => new InvoiceTaskItemViewModel(t))
            .Where(vm => vm.HasUnbilledHours)
            .OrderBy(vm => vm.StoryName)
            .ThenBy(vm => vm.TaskName)
            .ToList();

        TasksList.ItemsSource = allTasks;
    }

    private void FilterInput_TextChanged(object sender, TextChangedEventArgs e) {
        var filter = FilterInput.Text?.Trim() ?? string.Empty;

        if (string.IsNullOrEmpty(filter)) {
            TasksList.ItemsSource = allTasks;
            return;
        }

        var filtered = allTasks.Where(vm => {
            try {
                var pattern = Regex.Escape(filter);
                return Regex.IsMatch(vm.TaskName, pattern, RegexOptions.IgnoreCase) ||
                       Regex.IsMatch(vm.TaskDescription, pattern, RegexOptions.IgnoreCase);
            } catch {
                return vm.TaskName.Contains(filter, StringComparison.OrdinalIgnoreCase) ||
                       vm.TaskDescription.Contains(filter, StringComparison.OrdinalIgnoreCase);
            }
        }).ToList();

        TasksList.ItemsSource = filtered;
    }

    private void RateOverride_LostFocus(object sender, RoutedEventArgs e) {
        if (sender is TextBox textBox && textBox.Tag is InvoiceTaskItemViewModel vm) {
            if (decimal.TryParse(textBox.Text, out var rate)) {
                vm.HourlyRateOverride = rate;
            } else {
                vm.HourlyRateOverride = null;
            }
        }
    }

    private void CreateInvoice_Click(object sender, RoutedEventArgs e) {
        // Navigate back or show success message
        var mainWindow = (MainWindow)this.VisualRoot!;
        
        if (!decimal.TryParse(HourlyRateInput.Text, out var hourlyRate) || hourlyRate <= 0) {
            mainWindow.ShowDialogNoWait($"Specify an hourly rate first");
            return;
        }

        var selectedTasks = allTasks.Where(vm => vm.IsSelected).ToList();
        if (selectedTasks.Count == 0) {
            mainWindow.ShowDialogNoWait($"Select tasks to be billed first.");
            return;
        }

        // Generate billing ID
        var now = DateTime.Now;
        var billingId = GenerateBillingId(project.InvoiceMarker, now);

        var csvLines = new List<string> {
            $"Invoice ID:,{billingId}",
            $"Date:,{now:yyyy-MM-dd}",
            $"Project:,{EscapeCsv(project.Name)}",
            "",
            "Task Name,Description,Hours,Hourly Rate,Total Amount"
        };

        decimal grandTotal = 0;
        decimal grandTotalHours = 0;

        var invoice = new Invoice {
            BillingId = billingId,
            CreationDate = now,
            HourlyRate = hourlyRate,
            ProjectId = project.Id
        };

        db.Invoices.Add(invoice);
        db.SaveChanges();

        foreach (var taskVm in selectedTasks) {
            var task = taskVm.Task;

            var invoiceTask = new InvoiceTask {
                InvoiceId = invoice.Id,
                TaskItemId = task.Id,
                HourlyRateOverride = taskVm.HourlyRateOverride
            };

            db.InvoiceTasks.Add(invoiceTask);

            var hours = taskVm.UnbilledHours.TotalHours;
            var rate = taskVm.HourlyRateOverride ?? hourlyRate;
            var total = (decimal)hours * rate;
            grandTotal += total;
            grandTotalHours += (decimal)hours;

            var taskName = EscapeCsv(taskVm.TaskName);
            var description = EscapeCsv(taskVm.TaskDescription);
            csvLines.Add(string.Create(CultureInfo.InvariantCulture, $"{taskName},{description},{hours:F2},{rate:F2},{total:F2}"));
            
            // LAST: mark this as billed.
            var unbilledSessions = task.WorkSessions.Where(w => !w.Billed && w.EndDate.HasValue).ToList();
            foreach (var session in unbilledSessions) {
                session.Billed = true;
            }
        }

        csvLines.Add("");

        csvLines.Add(string.Create(CultureInfo.InvariantCulture, $"Total:,,{grandTotalHours:F2},,{grandTotal:F2}"));

        invoice.TotalAmount = grandTotal;
        db.SaveChanges();

        var fileName = $"{billingId}.csv";

        // Backup existing file if it exists
        if (File.Exists(fileName)) {
            var backupName = $"{billingId}_backup_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
            File.Copy(fileName, backupName, true);
        }

        File.WriteAllLines(fileName, csvLines);

        mainWindow.ShowDialogNoWait($"Invoice created successfully! File saved as: {fileName}");
    }

    private string GenerateBillingId(string invoiceMarker, DateTime date) {
        var year = date.Year;
        var month = date.Month;

        // Count existing invoices for this project in the current month
        var existingCount = db.Invoices
            .Where(i => i.ProjectId == project.Id)
            .AsEnumerable()
            .Count(i => i.CreationDate.Year == year && i.CreationDate.Month == month);

        var sequenceNumber = existingCount + 1;

        return $"{invoiceMarker}-{year}-{month:D2}-{sequenceNumber:D2}";
    }

    private string EscapeCsv(string value) {
        if (string.IsNullOrEmpty(value)) return "";
        if (value.Contains(",") || value.Contains("\"") || value.Contains("\n")) {
            return $"\"{value.Replace("\"", "\"\"")}\"";
        }
        return value;
    }

    private void Cancel_Click(object sender, RoutedEventArgs e) {
        var mainWindow = (MainWindow)this.VisualRoot!;
        mainWindow.NavigateBack();
    }

    public override string ViewTitle => $"Create Invoices for {project.Name}";

    public override void OnBecameActive() {
        base.OnBecameActive();
        LoadTasks();
    }
}
