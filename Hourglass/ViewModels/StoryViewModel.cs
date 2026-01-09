using System;
using System.Linq;
using Hourglass.Controls;
using Hourglass.Models;

namespace Hourglass.ViewModels;

public class StoryViewModel {
    public Story Story { get; }

    public StoryViewModel(Story story) {
        Story = story;
    }

    public string Name => Story.Name;
    public string Description => Story.Description;
    public string LinkText => !string.IsNullOrEmpty(Story.HyperLink) ? $"Link: {Story.HyperLink}" : string.Empty;
    public bool HasLink => !string.IsNullOrEmpty(Story.HyperLink);

    public string SummaryText {
        get {
            var totalEstimated = TimeSpan.FromTicks(Story.Tasks.Sum(t => t.EstimatedHours.Ticks));
            var totalSpent = TimeSpan.FromTicks(Story.Tasks.Sum(t =>
                t.WorkSessions.Where(w => w.EndDate.HasValue).Sum(w => (w.EndDate!.Value - w.StartDate).Ticks)));
            return $"Total Estimated: {TimeSpanInput.FormatTimeSpan(totalEstimated)} | Total Spent: {TimeSpanInput.FormatTimeSpan(totalSpent)}";
        }
    }
}
