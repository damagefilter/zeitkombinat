using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Microsoft.EntityFrameworkCore;
using Hourglass.Models;
using Hourglass.ViewModels;

namespace Hourglass.Views;

public partial class TasksView : HourglassControl {
    public Story Story { get; private set; }
    
    /// <summary>
    /// This is a constructor to make this view accessible for some automated and helpy systems
    /// </summary>
    public TasksView() {
        InitializeComponent();
        Story = new Story();
    }
    
    public TasksView(Story story) {
        InitializeComponent();
        Story = story;
        LoadStoryDetails();
        LoadTasks();
    }

    private void LoadStoryDetails() {
        RefreshStoryData();
        
        StoryName.Text = Story.Name;
        StoryDescription.Text = Story.Description;
        StoryLinkLabel.Text = !string.IsNullOrEmpty(Story.HyperLink) ? $"Link: {Story.HyperLink}" : string.Empty;
        Uri.TryCreate(Story.HyperLink, UriKind.Absolute, out var uri);
        StoryLinkButton.NavigateUri = uri;
        StoryLinkButton.IsVisible = !string.IsNullOrEmpty(Story.HyperLink);
    }

    private void RefreshStoryData() {
        Story = db.Stories
            .Include(s => s.Project)
            .Include(s => s.Tasks)
            .ThenInclude(t => t.WorkSessions)
            .First(s => s.Id == Story.Id);
    }

    private void LoadTasks() {
        TasksList.ItemsSource = Story.Tasks.Select(t => new TaskViewModel(t)).ToList();
    }

    private void AddTask_Click(object sender, RoutedEventArgs e) {
        var name = TaskNameInput.Text?.Trim();
        var description = DescriptionInput.Text?.Trim();
        var hyperLink = HyperLinkInput.Text?.Trim();

        if (string.IsNullOrEmpty(name)) return;

        var task = new TaskItem {
            Name = name,
            Description = description ?? string.Empty,
            HyperLink = hyperLink ?? string.Empty,
            EstimatedHours = EstimatedInput.Value,
            StoryId = Story.Id
        };

        Story.Tasks.Add(task);
        db.Tasks.Add(task);
        db.SaveChanges();

        TaskNameInput.Text = string.Empty;
        DescriptionInput.Text = string.Empty;
        HyperLinkInput.Text = string.Empty;
        EstimatedInput.SetText(string.Empty);
        
        LoadTasks();
    }

    private void DeleteTask_Click(object sender, RoutedEventArgs e) {
        if (sender is Button button && button.Tag is TaskItem task) {
            db.Tasks.Remove(task);
            db.SaveChanges();

            Story.Tasks.Remove(task);
            LoadTasks();
        }
    }

    private void ViewTaskDetails_Click(object sender, RoutedEventArgs e) {
        if (sender is Button button && button.Tag is TaskItem task) {
            var mainWindow = (MainWindow)this.VisualRoot!;
            mainWindow.NavigateToTaskDetails(task);
        }
    }

    private void EditStory_Click(object sender, RoutedEventArgs e) {
        StoryNameEdit.Text = Story.Name;
        StoryDescriptionEdit.Text = Story.Description;
        StoryLinkEdit.Text = Story.HyperLink;

        StoryDisplayPanel.IsVisible = false;
        StoryEditPanel.IsVisible = true;
    }

    private void SaveStory_Click(object sender, RoutedEventArgs e) {
        var name = StoryNameEdit.Text?.Trim();
        if (string.IsNullOrEmpty(name)) return;

        var dbStory = db.Stories.Find(Story.Id);
        if (dbStory != null) {
            dbStory.Name = name;
            dbStory.Description = StoryDescriptionEdit.Text?.Trim() ?? string.Empty;
            dbStory.HyperLink = StoryLinkEdit.Text?.Trim() ?? string.Empty;
            db.SaveChanges();

            Story.Name = dbStory.Name;
            Story.Description = dbStory.Description;
            Story.HyperLink = dbStory.HyperLink;

            LoadStoryDetails();
        }

        StoryDisplayPanel.IsVisible = true;
        StoryEditPanel.IsVisible = false;
    }

    private void CancelStoryEdit_Click(object sender, RoutedEventArgs e) {
        StoryDisplayPanel.IsVisible = true;
        StoryEditPanel.IsVisible = false;
    }

    public override string ViewTitle => $"Story {Story.Name} in {Story.Project.Name}";

    public override void OnBecameActive() {
        base.OnBecameActive();
        RefreshStoryData();
        LoadStoryDetails();
        LoadTasks();
    }
}
