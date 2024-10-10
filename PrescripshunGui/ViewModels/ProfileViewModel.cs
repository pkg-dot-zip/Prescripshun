using PrescripshunLib.Models.User.Profile;

namespace PrescripshunGui.ViewModels;

public class ProfileViewModel(IProfile profile) : ViewModelBase
{
    public IProfile Profile { get; } = profile;
}