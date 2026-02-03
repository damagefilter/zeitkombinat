using System;
using System.Linq;
using Zeitkombinat.Controls;
using Zeitkombinat.Models;

namespace Zeitkombinat.ViewModels;

public class TaskViewModel {
    public TaskItem Task { get; }

    public TaskViewModel(TaskItem task) {
        Task = task;
    }

    public string Name => Task.Name;
    public string Description => Task.Description;
    public string RawLink => Task.HyperLink;
    public string LinkText => !string.IsNullOrEmpty(Task.HyperLink) ? $"Link: {Task.HyperLink}" : string.Empty;
    public bool HasLink => !string.IsNullOrEmpty(Task.HyperLink);
    public bool HasOpenSessions => Task.WorkSessions.Any(x => !x.EndDate.HasValue);
    public bool IsDone => Task.IsDone;
    
    public string OpenSessionsDuration {
        get {
            DateTime now = DateTime.Now;
            
            double hours = Task.WorkSessions.Sum(x => {
                if (!x.EndDate.HasValue) {
                    return (now - x.StartDate).TotalHours;
                }

                return 0;
            });
            return TimeSpanInput.FormatTimeSpan(TimeSpan.FromHours(hours));
        }
    }

    public string HoursText {
        get {
            var totalSpent = TimeSpan.FromTicks(Task.WorkSessions.Where(w => w.EndDate.HasValue).Sum(w => (w.EndDate!.Value - w.StartDate).Ticks));
            return $"Estimated: {TimeSpanInput.FormatTimeSpan(Task.EstimatedHours)} | Spent: {TimeSpanInput.FormatTimeSpan(totalSpent)}";
        }
    }
    
    public string TotalEstimatedTime => $"Total Estimated: {TimeSpanInput.FormatTimeSpan(Task.EstimatedHours)}";

    public string TotalSpentTime {
        get {
            var totalSpent = TimeSpan.FromTicks(Task.WorkSessions
                .Where(w => w.EndDate.HasValue)
                .Sum(w => (w.EndDate!.Value - w.StartDate).Ticks));
            return $"Total Spent: {TimeSpanInput.FormatTimeSpan(totalSpent)}";
        }
    }
    
    public string TotalUnbilledTime {
        get {
            var totalUnbilled = TimeSpan.FromTicks(Task.WorkSessions
                .Where(w => w.EndDate.HasValue && !w.Billed)
                .Sum(w => (w.EndDate!.Value - w.StartDate).Ticks));
            return $"Total Unbilled: {TimeSpanInput.FormatTimeSpan(totalUnbilled)}";
        }
    }
}
