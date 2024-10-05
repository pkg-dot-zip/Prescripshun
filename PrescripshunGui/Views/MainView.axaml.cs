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
}
