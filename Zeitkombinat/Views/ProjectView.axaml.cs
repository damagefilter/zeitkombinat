using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Microsoft.EntityFrameworkCore;
using Zeitkombinat.Models;
using Zeitkombinat.ViewModels;

namespace Zeitkombinat.Views;

public partial class ProjectView : ZeitkombinatControl {
    public Project Project { get; private set; }

    /// <summary>
    /// This is a constructor to make this view accessible for some automated and helpy systems
    /// </summary>
    public ProjectView() {
        InitializeComponent();
        Project = new Project();
    }
    
    public ProjectView(Project project) {
        InitializeComponent();
        Project = project;
        RefreshProjectData();
        LoadAllData();
    }

    private void RefreshProjectData() {
        Project = db.Projects
            .Include(p => p.Stories)
            .ThenInclude(s => s.Tasks)
            .ThenInclude(t => t.WorkSessions)
            .First(p => p.Id == Project.Id);
    }

    private void LoadProjectDetails() {
        ProjectName.Text = Project.Name;
        ProjectDescription.Text = Project.Description;
        ProjectInvoiceMarker.Text = !string.IsNullOrEmpty(Project.InvoiceMarker) ? $"Invoice Marker: {Project.InvoiceMarker}" : string.Empty;
    }

    private void LoadStories() {
        StoriesList.ItemsSource = Project.Stories.Select(s => new StoryViewModel(s)).ToList();
    }

    private void LoadInvoices() {
        var invoices = db.Invoices
            .Where(i => i.ProjectId == Project.Id)
            .OrderByDescending(i => i.CreationDate)
            .ToList();

        InvoicesList.ItemsSource = invoices.Select(i => new InvoiceViewModel(i)).ToList();
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
        db.Stories.Add(story);
        db.SaveChanges();

        StoryNameInput.Text = string.Empty;
        DescriptionInput.Text = string.Empty;
        HyperLinkInput.Text = string.Empty;
        
        LoadStories();
        
        Button_OnClickShowAddStory(null, new RoutedEventArgs());
    }

    private void DeleteStory_Click(object sender, RoutedEventArgs e) {
        if (sender is Button button && button.Tag is Story story) {
            db.Stories.Remove(story);
            db.SaveChanges();

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
        ProjectCurrencyEdit.Text = Project.CurrencySymbol;

        ProjectDisplayPanel.IsVisible = false;
        ProjectEditPanel.IsVisible = true;
    }

    private void SaveProject_Click(object sender, RoutedEventArgs e) {
        var name = ProjectNameEdit.Text?.Trim();
        if (string.IsNullOrEmpty(name)) return;

        var dbProject = db.Projects.Find(Project.Id);
        if (dbProject != null) {
            dbProject.Name = name;
            dbProject.Description = ProjectDescriptionEdit.Text?.Trim() ?? string.Empty;
            dbProject.InvoiceMarker = ProjectInvoiceMarkerEdit.Text?.Trim() ?? string.Empty;
            dbProject.CurrencySymbol = ProjectCurrencyEdit.Text?.Trim() ?? "€";
            db.SaveChanges();

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

    private void CreateInvoice_Click(object sender, RoutedEventArgs e) {
        var mainWindow = (MainWindow)this.VisualRoot!;
        mainWindow.NavigateToCreateInvoice(Project);
    }

    public override string ViewTitle => $"Project {Project.Name}";

    public override void OnBecameActive() {
        base.OnBecameActive();
        LoadAllData();
    }

    private void LoadAllData() {
        RefreshProjectData();
        LoadProjectDetails();
        LoadStories();
        LoadInvoices();
    }

    private void Button_OnClickShowAddStory(object? sender, RoutedEventArgs e) {
        AddStoryForm.IsVisible = !AddStoryForm.IsVisible;
        ToggleAddStoryButton.Content = AddStoryForm.IsVisible ? "Hide" : "Add Story";
    }
}
