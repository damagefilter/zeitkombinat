using System;
using System.Linq;
using Zeitkombinat.Controls;
using Zeitkombinat.Models;

namespace Zeitkombinat.ViewModels;

public class InvoiceTaskItemViewModel {
    public TaskItem Task { get; }
    public bool IsSelected { get; set; }
    public decimal? HourlyRateOverride { get; set; }

    public InvoiceTaskItemViewModel(TaskItem task) {
        Task = task;
    }

    public string TaskName => Task.Name;
    public string TaskDescription => Task.Description;
    public string StoryName => Task.Story?.Name ?? "Unknown Story";

    public TimeSpan UnbilledHours {
        get {
            var unbilledTicks = Task.WorkSessions
                .Where(w => !w.Billed && w.EndDate.HasValue)
                .Sum(w => (w.EndDate!.Value - w.StartDate).Ticks);
            return TimeSpan.FromTicks(unbilledTicks);
        }
    }

    public string UnbilledHoursText => TimeSpanInput.FormatTimeSpan(UnbilledHours);

    public bool HasUnbilledHours => UnbilledHours.TotalSeconds > 0;
}
