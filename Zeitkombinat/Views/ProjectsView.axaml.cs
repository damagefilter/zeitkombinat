using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Zeitkombinat;
using Zeitkombinat.Models;

namespace Zeitkombinat.Views;

public partial class ProjectsView : ZeitkombinatControl {

    public ProjectsView() {
        InitializeComponent();
        LoadProjects();
    }

    private void LoadProjects() {
        var projects = db.Projects.ToList();
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

        db.Projects.Add(project);
        db.SaveChanges();

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
            db.Projects.Remove(project);
            db.SaveChanges();

            LoadProjects();
        }
    }

    private void CreateInvoice_Click(object sender, RoutedEventArgs e) {
        if (sender is Button button && button.Tag is Project project) {
            var mainWindow = (MainWindow)this.VisualRoot!;
            mainWindow.NavigateToCreateInvoice(project);
        }
    }

    public override string ViewTitle => "Overview";

    public override void OnBecameActive() {
        base.OnBecameActive();
        LoadProjects();
    }
}
