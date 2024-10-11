using PrescripshunLib.Models.MedicalFile;
using PrescripshunLib.Models.User.Profile;

namespace PrescripshunGui.ViewModels;

public class ProfileViewModel : ViewModelBase
{
    public IProfile Profile { get; }
    public IMedicalFile MedicalFile { get; }

    public ProfileViewModel(IProfile profile, IMedicalFile medicalFile)
    {
        Profile = profile;
        MedicalFile = medicalFile;
    }
}