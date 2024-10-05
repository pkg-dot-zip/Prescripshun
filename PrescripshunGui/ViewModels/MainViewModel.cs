using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using PrescripshunGui.Util;
using PrescripshunLib.Networking.Messages;

namespace PrescripshunGui.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    public string WelcomeText => "Prescripshun";

    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    public ICommand LoginCommand { get; private set; }

    public MainViewModel()
    {
        LoginCommand = new AsyncRelayCommand(async () =>
        {
            await NetworkHandler.Send(new Message.DebugPrint()
            {
                Text = "Can send stuff from the GUI!",
            });

            await NetworkHandler.Send(new LoginRequest()
            {
                Username = Username,
                Password = Password,
            });
        });
    }
}
