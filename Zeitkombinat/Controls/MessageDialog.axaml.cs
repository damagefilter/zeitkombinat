using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Zeitkombinat.Controls;

public partial class MessageDialog : Window {
    public bool Result { get; private set; }

    public MessageDialog() {
        InitializeComponent();
    }

    public static MessageDialog CreateMessage(string message, string title = "Message", bool showCancel = false) {
        var dialog = new MessageDialog {
            Title = title
        };
        dialog.MessageTextBlock.Text = message;
        dialog.CancelButton.IsVisible = showCancel;
        return dialog;
    }

    private void OkButton_Click(object sender, RoutedEventArgs e) {
        Result = true;
        Close();
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e) {
        Result = false;
        Close();
    }
}
