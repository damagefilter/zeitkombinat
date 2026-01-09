using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Hourglass.Data;

namespace Hourglass;

public partial class App : Application {
    public override void Initialize() {
        AvaloniaXamlLoader.Load(this);
        InitializeDatabase();
    }

    private void InitializeDatabase() {
        using var db = new HourglassDbContext();
        db.Database.EnsureCreated();
    }

    public override void OnFrameworkInitializationCompleted() {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop) {
            desktop.MainWindow = new MainWindow();
        }

        base.OnFrameworkInitializationCompleted();
    }
}