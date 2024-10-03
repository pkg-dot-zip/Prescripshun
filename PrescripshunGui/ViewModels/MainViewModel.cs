using System;
using System.Windows.Input;
using Avalonia;
using Avalonia.Interactivity;
using CommunityToolkit.Mvvm.Input;

namespace PrescripshunGui.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    public string WelcomeText => "Prescripshun";

    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
