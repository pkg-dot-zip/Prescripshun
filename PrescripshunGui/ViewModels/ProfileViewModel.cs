using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using PrescripshunGui.Util;
using PrescripshunLib.Models.MedicalFile;
using PrescripshunLib.Models.User.Profile;
using PrescripshunLib.Networking.Messages;

namespace PrescripshunGui.ViewModels;

public class ProfileViewModel : ViewModelBase
{
    public Profile Profile { get; }
    public MedicalFile MedicalFile { get; set; }
    private readonly IDatabaseHandler _databaseHandler;

    public ProfileViewModel(Profile profile, IDatabaseHandler databaseHandler)
    {
        Profile = profile;
        _databaseHandler = databaseHandler;
    }

    public async Task GetMedicalFile(Guid guid)
    {
        MedicalFile = await _databaseHandler.GetMedicalFile(guid);
    }
}
