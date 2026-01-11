using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Hourglass.Data;
using Microsoft.EntityFrameworkCore;

namespace Hourglass;

public partial class App : Application {
    public override void Initialize() {
        AvaloniaXamlLoader.Load(this);
        InitializeDatabase();
    }

    private void InitializeDatabase() {
        using var db = new HourglassDbContext();
        db.Database.Migrate(); // Ensure we have everything and schema is up to date
    }

    public override void OnFrameworkInitializationCompleted() {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop) {
            desktop.MainWindow = new MainWindow();
        }

        base.OnFrameworkInitializationCompleted();
    }
}