using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Zeitkombinat;
using Zeitkombinat.Controls;
using Zeitkombinat.Models;
using Zeitkombinat.Views;

namespace Zeitkombinat;

public partial class MainWindow : Window {
    private readonly Stack<ZeitkombinatControl> _navigationStack = new();

    public MainWindow() {
        InitializeComponent();
        NavigateToProjects();
    }

    public void NavigateToProjects() {
        _navigationStack.Clear();
        var view = new Overview();
        PushView(view);
    }

    public void NavigateToStories(Project project) {
        var view = new ProjectView(project);
        PushView(view);
    }

    public void NavigateToTasks(Story story) {
        var view = new TasksView(story);
        PushView(view);
    }

    public void NavigateToTaskDetails(TaskItem task) {
        var view = new TaskDetailsView(task);
        PushView(view);
    }

    public void NavigateToCreateInvoice(Project project) {
        var view = new CreateInvoiceView(project);
        PushView(view);
    }

    public async Task<bool> ShowDialog(string message, string title = "Message", bool showCancel = false) {
        var dialog = MessageDialog.CreateMessage(message, title, showCancel);
        await dialog.ShowDialog(this);
        return dialog.Result;
    }
    
    public async void ShowDialogNoWait(string message, string title = "Message", bool showCancel = false) {
        var dialog = MessageDialog.CreateMessage(message, title, showCancel);
        await dialog.ShowDialog(this);
    }

    public void NavigateBack() {
        BackButton_Click(this, new RoutedEventArgs());
    }

    private void PushView(ZeitkombinatControl view) {
        _navigationStack.Push(view);
        MainContent.Content = view;
        TitleTextBlock.Text = view.ViewTitle;
        BackButton.IsVisible = _navigationStack.Count > 1;
    }

    private void BackButton_Click(object sender, RoutedEventArgs e) {
        if (_navigationStack.Count > 1) {
            _navigationStack.Pop();
            var view = _navigationStack.Peek();
            MainContent.Content = view;
            BackButton.IsVisible = _navigationStack.Count > 1;
            view.OnBecameActive();
            TitleTextBlock.Text = view.ViewTitle;
        }
    }
}