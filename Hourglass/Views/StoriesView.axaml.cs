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

    /// <summary>
    /// This is a constructor to make this view accessible for some automated and helpy systems
    /// </summary>
    public StoriesView() {
        InitializeComponent();
        Project = new Project();
    }
    
    public StoriesView(Project project) {
        InitializeComponent();
        Project = _db.Projects
            .Include(p => p.Stories)
            .ThenInclude(s => s.Tasks)
            .ThenInclude(t => t.WorkSessions)
            .First(p => p.Id == project.Id);

        LoadProjectDetails();
        LoadStories();
    }

    private void LoadProjectDetails() {
        ProjectName.Text = Project.Name;
        ProjectDescription.Text = Project.Description;
        ProjectInvoiceMarker.Text = !string.IsNullOrEmpty(Project.InvoiceMarker) ? $"Invoice Marker: {Project.InvoiceMarker}" : string.Empty;
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

        Project.Stories.Add(story);
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

    private void EditProject_Click(object sender, RoutedEventArgs e) {
        ProjectNameEdit.Text = Project.Name;
        ProjectDescriptionEdit.Text = Project.Description;
        ProjectInvoiceMarkerEdit.Text = Project.InvoiceMarker;

        ProjectDisplayPanel.IsVisible = false;
        ProjectEditPanel.IsVisible = true;
    }

    private void SaveProject_Click(object sender, RoutedEventArgs e) {
        var name = ProjectNameEdit.Text?.Trim();
        if (string.IsNullOrEmpty(name)) return;

        var dbProject = _db.Projects.Find(Project.Id);
        if (dbProject != null) {
            dbProject.Name = name;
            dbProject.Description = ProjectDescriptionEdit.Text?.Trim() ?? string.Empty;
            dbProject.InvoiceMarker = ProjectInvoiceMarkerEdit.Text?.Trim() ?? string.Empty;
            _db.SaveChanges();

            Project.Name = dbProject.Name;
            Project.Description = dbProject.Description;
            Project.InvoiceMarker = dbProject.InvoiceMarker;

            LoadProjectDetails();
        }

        ProjectDisplayPanel.IsVisible = true;
        ProjectEditPanel.IsVisible = false;
    }

    private void CancelProjectEdit_Click(object sender, RoutedEventArgs e) {
        ProjectDisplayPanel.IsVisible = true;
        ProjectEditPanel.IsVisible = false;
    }
}
