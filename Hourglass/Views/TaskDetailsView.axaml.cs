using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Microsoft.EntityFrameworkCore;
using Hourglass.Controls;
using Hourglass.Data;
using Hourglass.Models;

namespace Hourglass.Views;

public partial class TaskDetailsView : UserControl {
    private readonly HourglassDbContext _db = new();
    public TaskItem TaskItem { get; }

    public TaskDetailsView(TaskItem task) {
        InitializeComponent();
        TaskItem = _db.Tasks.Include(t => t.WorkSessions).First(t => t.Id == task.Id);
        LoadTaskDetails();
    }

    private void LoadTaskDetails() {
        TaskTitle.Text = TaskItem.Name;
        TaskDescription.Text = TaskItem.Description;
        TaskLink.Text = !string.IsNullOrEmpty(TaskItem.HyperLink) ? $"Link: {TaskItem.HyperLink}" : string.Empty;

        var totalSpent = TimeSpan.FromTicks(TaskItem.WorkSessions.Where(w => w.EndDate.HasValue).Sum(w => (w.EndDate!.Value - w.StartDate).Ticks));
        TaskHours.Text = $"Estimated: {TimeSpanInput.FormatTimeSpan(TaskItem.EstimatedHours)} | Spent: {TimeSpanInput.FormatTimeSpan(totalSpent)}";

        LoadSessions();
    }

    private void LoadSessions() {
        var openSessions = TaskItem.WorkSessions.Where(w => !w.EndDate.HasValue).Select(w => new ViewModels.WorkSessionViewModel(w)).ToList();
        var completedSessions = TaskItem.WorkSessions.Where(w => w.EndDate.HasValue).OrderByDescending(w => w.StartDate).Select(w => new ViewModels.WorkSessionViewModel(w)).ToList();

        OpenSessionsList.ItemsSource = openSessions;
        CompletedSessionsList.ItemsSource = completedSessions;
    }

    private void StartSession_Click(object sender, RoutedEventArgs e) {
        var session = new WorkSession {
            StartDate = DateTime.Now,
            TaskItemId = TaskItem.Id
        };

        TaskItem.WorkSessions.Add(session);
        _db.WorkSessions.Add(session);
        _db.SaveChanges();
        LoadSessions();
    }

    private void DeleteSession_Click(object sender, RoutedEventArgs e) {
        if (sender is Button button && button.Tag is WorkSession session) {
            _db.WorkSessions.Remove(session);
            _db.SaveChanges();

            TaskItem.WorkSessions.Remove(session);
            LoadTaskDetails();
        }
    }

    private void CloseSession_Click(object sender, RoutedEventArgs e) {
        if (sender is Button button && button.Tag is WorkSession session) {
            var dbSession = _db.WorkSessions.Find(session.Id);
            if (dbSession != null) {
                dbSession.EndDate = DateTime.Now;
                _db.SaveChanges();

                // Refresh from database using a fresh query to avoid duplication
                var refreshedTask = _db.Tasks
                    .Include(t => t.WorkSessions)
                    .First(t => t.Id == TaskItem.Id);
                
                TaskItem.WorkSessions.Clear();
                foreach (var ws in refreshedTask.WorkSessions) {
                    TaskItem.WorkSessions.Add(ws);
                }
                
                LoadTaskDetails();
            }
        }
    }

    private void AddCompletedSession_Click(object sender, RoutedEventArgs e) {
        var duration = DurationInput.Value;
        
        if (duration.TotalSeconds <= 0) return;

        var endDate = DateTime.Now;
        var startDate = endDate.Subtract(duration);

        var session = new WorkSession {
            StartDate = startDate,
            EndDate = endDate,
            TaskItemId = TaskItem.Id
        };

        _db.WorkSessions.Add(session);
        _db.SaveChanges();

        DurationInput.SetText(string.Empty);

        // Refresh from database using a fresh query to avoid duplication
        var refreshedTask = _db.Tasks
            .Include(t => t.WorkSessions)
            .First(t => t.Id == TaskItem.Id);
        
        TaskItem.WorkSessions.Clear();
        foreach (var ws in refreshedTask.WorkSessions) {
            TaskItem.WorkSessions.Add(ws);
        }
        
        LoadTaskDetails();
    }
}
