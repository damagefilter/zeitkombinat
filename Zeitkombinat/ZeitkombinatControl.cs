using Avalonia.Controls;
using Zeitkombinat.Data;

namespace Zeitkombinat;

public abstract class ZeitkombinatControl : UserControl {
    protected ZeitkombinatDbContext db = new();
    
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
        db = new ZeitkombinatDbContext();
    }
}