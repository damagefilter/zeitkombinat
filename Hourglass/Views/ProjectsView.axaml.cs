using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Microsoft.EntityFrameworkCore;
using Hourglass.Data;
using Hourglass.Models;

namespace Hourglass.Views;

public partial class ProjectsView : UserControl {
    private readonly HourglassDbContext _db = new();

    public ProjectsView() {
        InitializeComponent();
        LoadProjects();
    }

    private void LoadProjects() {
        var projects = _db.Projects.ToList();
        ProjectsList.ItemsSource = projects;
    }

    private void AddProject_Click(object sender, RoutedEventArgs e) {
        var name = ProjectNameInput.Text?.Trim();
        var invoiceMarker = InvoiceMarkerInput.Text?.Trim();
        var description = DescriptionInput.Text?.Trim();

        if (string.IsNullOrEmpty(name)) {
            return;
        }

        var project = new Project {
            Name = name,
            InvoiceMarker = invoiceMarker ?? string.Empty,
            Description = description ?? string.Empty
        };

        _db.Projects.Add(project);
        _db.SaveChanges();

        ProjectNameInput.Text = string.Empty;
        InvoiceMarkerInput.Text = string.Empty;
        DescriptionInput.Text = string.Empty;

        LoadProjects();
    }

    private void ViewStories_Click(object sender, RoutedEventArgs e) {
        if (sender is Button button && button.Tag is Project project) {
            var mainWindow = (MainWindow)this.VisualRoot!;
            mainWindow.NavigateToStories(project);
        }
    }

    private void DeleteProject_Click(object sender, RoutedEventArgs e) {
        if (sender is Button button && button.Tag is Project project) {
            _db.Projects.Remove(project);
            _db.SaveChanges();

            LoadProjects();
        }
    }
}
