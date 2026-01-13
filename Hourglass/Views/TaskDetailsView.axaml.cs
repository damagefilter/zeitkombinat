using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Microsoft.EntityFrameworkCore;
using Hourglass.Controls;
using Hourglass.Models;

namespace Hourglass.Views;

public partial class TaskDetailsView : HourglassControl {
    public TaskItem TaskItem { get; private set; }

    /// <summary>
    /// Automation ctor
    /// </summary>
    public TaskDetailsView() {
        InitializeComponent();
        TaskItem = new TaskItem();
    }
    public TaskDetailsView(TaskItem task) {
        InitializeComponent();
        // This might already be fully loaded.
        // But so far performance is fine the way it is and this is safer.
        TaskItem = task;
        LoadTaskDetails();
    }

    private void LoadTaskDetails() {
        TaskItem = db.Tasks
            .Include(t => t.WorkSessions)
            .Include(t => t.Story)
            .ThenInclude(s => s.Project)
            .First(t => t.Id == TaskItem.Id);
        
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
        db.WorkSessions.Add(session);
        db.SaveChanges();
        LoadSessions();
    }

    private void DeleteSession_Click(object sender, RoutedEventArgs e) {
        if (sender is Button button && button.Tag is WorkSession session) {
            db.WorkSessions.Remove(session);
            db.SaveChanges();
            TaskItem.WorkSessions.Remove(session);
            LoadTaskDetails();
        }
    }

    private void CloseSession_Click(object sender, RoutedEventArgs e) {
        if (sender is Button button && button.Tag is WorkSession session) {
            var dbSession = db.WorkSessions.Find(session.Id);
            if (dbSession != null) {
                dbSession.EndDate = DateTime.Now;
                db.SaveChanges();
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

        db.WorkSessions.Add(session);
        db.SaveChanges();

        DurationInput.SetText(string.Empty);

        LoadTaskDetails();
    }

    private void EditTask_Click(object sender, RoutedEventArgs e) {
        TaskNameEdit.Text = TaskItem.Name;
        TaskDescriptionEdit.Text = TaskItem.Description;
        TaskLinkEdit.Text = TaskItem.HyperLink;
        TaskEstimatedEdit.Value = TaskItem.EstimatedHours;

        TaskDisplayPanel.IsVisible = false;
        TaskEditPanel.IsVisible = true;
    }

    private void SaveTask_Click(object sender, RoutedEventArgs e) {
        var name = TaskNameEdit.Text?.Trim();
        if (string.IsNullOrEmpty(name)) return;

        var dbTask = db.Tasks.Find(TaskItem.Id);
        if (dbTask != null) {
            dbTask.Name = name;
            dbTask.Description = TaskDescriptionEdit.Text?.Trim() ?? string.Empty;
            dbTask.HyperLink = TaskLinkEdit.Text?.Trim() ?? string.Empty;
            dbTask.EstimatedHours = TaskEstimatedEdit.Value;
            db.SaveChanges();

            TaskItem.Name = dbTask.Name;
            TaskItem.Description = dbTask.Description;
            TaskItem.HyperLink = dbTask.HyperLink;
            TaskItem.EstimatedHours = dbTask.EstimatedHours;

            LoadTaskDetails();
        }

        TaskDisplayPanel.IsVisible = true;
        TaskEditPanel.IsVisible = false;
    }

    private void CancelTaskEdit_Click(object sender, RoutedEventArgs e) {
        TaskDisplayPanel.IsVisible = true;
        TaskEditPanel.IsVisible = false;
    }

    public override string ViewTitle => $"Work Sessions for {TaskItem.Name} in {TaskItem.Story.Name} of {TaskItem.Story.Project.Name}";

    public override void OnBecameActive() {
        base.OnBecameActive();
        LoadTaskDetails();
    }
}
