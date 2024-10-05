using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using PrescripshunLib.ExtensionMethods;
using PrescripshunLib.Networking.Messages;
using NetworkHandler = PrescripshunGui.Util.NetworkHandler;

namespace PrescripshunGui.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
    }

    public async void ClickHandler(object sender, RoutedEventArgs args)
    {
        LoginButton.IsVisible = false;
        await NetworkHandler.Send(new Message.DebugPrint()
        {
            Text = "Can send stuff from the GUI!",
        });

        await NetworkHandler.Send(new LoginRequest()
        {
            Username = "Test",
            Password = "Wajoow",
        });
    }
}
