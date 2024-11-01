using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using PrescripshunGui.ViewModels;

namespace PrescripshunGui.Views;

public partial class ProfileView : UserControl
{
    public ProfileView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
