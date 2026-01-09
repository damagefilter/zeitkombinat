using System;
using System.Text.RegularExpressions;
using Avalonia;
using Avalonia.Controls;

namespace Hourglass.Controls;

public partial class TimeSpanInput : UserControl {
    public static readonly StyledProperty<TimeSpan> ValueProperty =
        AvaloniaProperty.Register<TimeSpanInput, TimeSpan>(nameof(Value));

    public TimeSpan Value {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public TimeSpanInput() {
        InitializeComponent();
    }

    private void InputBox_TextChanged(object sender, TextChangedEventArgs e) {
        var text = InputBox.Text?.Trim() ?? string.Empty;
        Value = ParseTimeSpan(text);
    }

    public void SetText(string text) {
        InputBox.Text = text;
    }

    public string GetText() => InputBox.Text ?? string.Empty;

    private TimeSpan ParseTimeSpan(string input) {
        if (string.IsNullOrWhiteSpace(input)) return TimeSpan.Zero;

        var hours = 0;
        var minutes = 0;

        var hoursMatch = Regex.Match(input, @"(\d+)h", RegexOptions.IgnoreCase);
        if (hoursMatch.Success) {
            hours = int.Parse(hoursMatch.Groups[1].Value);
        }

        var minutesMatch = Regex.Match(input, @"(\d+)m", RegexOptions.IgnoreCase);
        if (minutesMatch.Success) {
            minutes = int.Parse(minutesMatch.Groups[1].Value);
        }

        return new TimeSpan(hours, minutes, 0);
    }

    public static string FormatTimeSpan(TimeSpan ts) {
        if (ts == TimeSpan.Zero) return "0h";
        var parts = new System.Collections.Generic.List<string>();
        if (ts.Hours > 0 || ts.Days > 0) parts.Add($"{(int)ts.TotalHours}h");
        if (ts.Minutes > 0) parts.Add($"{ts.Minutes}m");
        return string.Join(" ", parts);
    }
}
