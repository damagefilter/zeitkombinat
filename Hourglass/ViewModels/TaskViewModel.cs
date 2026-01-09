using System;
using System.Linq;
using Hourglass.Controls;
using Hourglass.Models;

namespace Hourglass.ViewModels;

public class TaskViewModel {
    public TaskItem Task { get; }

    public TaskViewModel(TaskItem task) {
        Task = task;
    }

    public string Name => Task.Name;
    public string Description => Task.Description;
    public string LinkText => !string.IsNullOrEmpty(Task.HyperLink) ? $"Link: {Task.HyperLink}" : string.Empty;
    public bool HasLink => !string.IsNullOrEmpty(Task.HyperLink);

    public string HoursText {
        get {
            var totalSpent = TimeSpan.FromTicks(Task.WorkSessions.Where(w => w.EndDate.HasValue).Sum(w => (w.EndDate!.Value - w.StartDate).Ticks));
            return $"Estimated: {TimeSpanInput.FormatTimeSpan(Task.EstimatedHours)} | Spent: {TimeSpanInput.FormatTimeSpan(totalSpent)}";
        }
    }
}
