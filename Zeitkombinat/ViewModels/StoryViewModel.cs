using System;
using System.Linq;
using Zeitkombinat.Controls;
using Zeitkombinat.Models;

namespace Zeitkombinat.ViewModels;

public class StoryViewModel {
    public Story Story { get; }

    public StoryViewModel(Story story) {
        Story = story;
    }

    public string Name => Story.Name;
    public string Description => Story.Description;
    public string RawLink => Story.HyperLink;
    public string LinkText => !string.IsNullOrEmpty(Story.HyperLink) ? $"Link: {Story.HyperLink}" : string.Empty;
    public bool HasLink => !string.IsNullOrEmpty(Story.HyperLink);

    public string TotalEstimatedTime {
        get {
            var totalEstimated = TimeSpan.FromTicks(Story.Tasks.Sum(t => t.EstimatedHours.Ticks));
            return $"Total Estimated: {TimeSpanInput.FormatTimeSpan(totalEstimated)}";
        }
    }
    
    public string TotalSpentTime {
        get {
            var totalSpent = TimeSpan.FromTicks(Story.Tasks.Sum(t =>
                t.WorkSessions.Where(w => w.EndDate.HasValue).Sum(w => (w.EndDate!.Value - w.StartDate).Ticks)));
            return $"Total Spent: {TimeSpanInput.FormatTimeSpan(totalSpent)}";
        }
    }
    
    public string TotalUnbilledTime {
        get {
            var totalUnbilled = TimeSpan.FromTicks(Story.Tasks.Sum(t =>
                t.WorkSessions.Where(w => w.EndDate.HasValue && !w.Billed).Sum(w => (w.EndDate!.Value - w.StartDate).Ticks)));
            return $"Total Unbilled: {TimeSpanInput.FormatTimeSpan(totalUnbilled)}";
        }
    }
}
