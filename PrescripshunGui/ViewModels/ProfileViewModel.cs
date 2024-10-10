using PrescripshunLib.Models.User.Profile;

namespace PrescripshunGui.ViewModels;

public class ProfileViewModel : ViewModelBase
{
    public IProfile Profile { get; }

    public ProfileViewModel(IProfile profile)
    {
        Profile = profile;
    }
}