using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.EntityFrameworkCore;
using Zeitkombinat.Data;

namespace Zeitkombinat;

public partial class App : Application {
    public override void Initialize() {
        AvaloniaXamlLoader.Load(this);
        InitializeDatabase();
    }

    private void InitializeDatabase() {
        using var db = new ZeitkombinatDbContext();
        db.Database.Migrate(); // Ensure we have everything and schema is up to date
    }

    public override void OnFrameworkInitializationCompleted() {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop) {
            desktop.MainWindow = new MainWindow();
        }

        base.OnFrameworkInitializationCompleted();
    }
}