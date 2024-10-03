using Avalonia.Controls;
using Avalonia.Interactivity;

namespace PrescripshunGui.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
    }

    public void ClickHandler(object sender, RoutedEventArgs args)
    {
        LoginButton.IsVisible = false;
    }
}
