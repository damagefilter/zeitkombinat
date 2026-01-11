using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Hourglass.Controls;
using Hourglass.Models;
using Hourglass.Views;

namespace Hourglass;

public partial class MainWindow : Window {
    private readonly Stack<UserControl> _navigationStack = new();

    public MainWindow() {
        InitializeComponent();
        NavigateToProjects();
    }

    public void NavigateToProjects() {
        _navigationStack.Clear();
        var view = new ProjectsView();
        PushView(view, "Projects");
    }

    public void NavigateToStories(Project project) {
        var view = new StoriesView(project);
        PushView(view, $"Stories: {project.Name}");
    }

    public void NavigateToTasks(Story story) {
        var view = new TasksView(story);
        PushView(view, $"Tasks: {story.Name}");
    }

    public void NavigateToTaskDetails(TaskItem task) {
        var view = new TaskDetailsView(task);
        PushView(view, $"Task: {task.Name}");
    }

    public void NavigateToCreateInvoice(Project project) {
        var view = new CreateInvoiceView(project);
        PushView(view, $"Create Invoice: {project.Name}");
    }

    public async Task<bool> ShowMessage(string message, string title = "Message", bool showCancel = false) {
        var dialog = MessageDialog.CreateMessage(message, title, showCancel);
        await dialog.ShowDialog(this);
        return dialog.Result;
    }

    public void NavigateBack() {
        BackButton_Click(this, new RoutedEventArgs());
    }

    private void PushView(UserControl view, string title) {
        _navigationStack.Push(view);
        MainContent.Content = view;
        TitleTextBlock.Text = title;
        BackButton.IsVisible = _navigationStack.Count > 1;
    }

    private void BackButton_Click(object sender, RoutedEventArgs e) {
        if (_navigationStack.Count > 1) {
            _navigationStack.Pop();
            var view = _navigationStack.Peek();
            MainContent.Content = view;
            BackButton.IsVisible = _navigationStack.Count > 1;
            
            // Restore title
            if (view is ProjectsView) TitleTextBlock.Text = "Projects";
            else if (view is StoriesView sv) TitleTextBlock.Text = $"Stories: {sv.Project.Name}";
            else if (view is TasksView tv) TitleTextBlock.Text = $"Tasks: {tv.Story.Name}";
            else if (view is TaskDetailsView tdv) TitleTextBlock.Text = $"Task: {tdv.TaskItem.Name}";
            else if (view is CreateInvoiceView) TitleTextBlock.Text = "Create Invoice";
        }
    }
}