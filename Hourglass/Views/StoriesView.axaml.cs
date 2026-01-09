using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Microsoft.EntityFrameworkCore;
using Hourglass.Data;
using Hourglass.Models;
using Hourglass.ViewModels;

namespace Hourglass.Views;

public partial class StoriesView : UserControl {
    private readonly HourglassDbContext _db = new();
    public Project Project { get; }
    private readonly Project currentProject;

    /// <summary>
    /// This is a constructor to make this view accessible for some automated and helpy systems
    /// </summary>
    public StoriesView() {
        InitializeComponent();
    }
    
    public StoriesView(Project project) {
        InitializeComponent();
        currentProject = project;
        Project = _db.Projects
            .Include(p => p.Stories)
            .ThenInclude(s => s.Tasks)
            .ThenInclude(t => t.WorkSessions)
            .First(p => p.Id == currentProject.Id);
        
        LoadStories();
    }

    private void LoadStories() {
        StoriesList.ItemsSource = Project.Stories.Select(s => new StoryViewModel(s)).ToList();
    }

    private void AddStory_Click(object sender, RoutedEventArgs e) {
        var name = StoryNameInput.Text?.Trim();
        var description = DescriptionInput.Text?.Trim();
        var hyperLink = HyperLinkInput.Text?.Trim();

        if (string.IsNullOrEmpty(name)) {
            return;
        }

        var story = new Story {
            Name = name,
            Description = description ?? string.Empty,
            HyperLink = hyperLink ?? string.Empty,
            ProjectId = Project.Id
        };

        currentProject.Stories.Add(story);
        _db.Stories.Add(story);
        _db.SaveChanges();

        StoryNameInput.Text = string.Empty;
        DescriptionInput.Text = string.Empty;
        HyperLinkInput.Text = string.Empty;
        
        LoadStories();
    }

    private void DeleteStory_Click(object sender, RoutedEventArgs e) {
        if (sender is Button button && button.Tag is Story story) {
            _db.Stories.Remove(story);
            _db.SaveChanges();

            Project.Stories.Remove(story);
            LoadStories();
        }
    }

    private void ViewTasks_Click(object? sender, RoutedEventArgs e) {
        if (sender is Button button && button.Tag is Story story) {
            var mainWindow = (MainWindow)this.VisualRoot!;
            mainWindow.NavigateToTasks(story);
        }
    }
}
