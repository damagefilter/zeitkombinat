using Avalonia.Controls;
using Hourglass.Data;

namespace Hourglass;

public abstract class HourglassControl : UserControl {
    protected HourglassDbContext db = new();
    
    public abstract string ViewTitle { get; }

    /// <summary>
    /// Called from the main window view stack when an existing view is becoming active again.
    /// </summary>
    public virtual void OnBecameActive() {
        RefreshDbContext();
    }

    public void OnDispose() {
        db.Dispose();
    }

    protected void RefreshDbContext() {
        db.Dispose();
        db = new HourglassDbContext();
    }
}