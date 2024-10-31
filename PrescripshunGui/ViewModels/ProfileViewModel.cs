using PrescripshunGui.ViewModels;
using PrescripshunLib.Models.MedicalFile;
using PrescripshunLib.Models.User.Profile;
using System.Threading.Tasks;
using System;
using PrescripshunClient;
using PrescripshunLib.Networking.Messages;
using Unclassified.Net;
using PrescripshunGui.Util;

public class ProfileViewModel : ViewModelBase
{
    public Profile Profile { get; }
    public MedicalFile MedicalFile { get; private set; }
    public MedicalFileHandler MedicalFileHandler;

    private readonly Client _client;

    public ProfileViewModel(Profile profile, Guid userkey, Client client)
    {
        Profile = profile;
        _client = client;
        InitializeAsync(userkey);
        GuiEvents.Get.RegisterMedicalFileCallback(OnMedicalFileReceived);
    }

    private async Task InitializeAsync(Guid userKey)
    {
        MedicalFileHandler MedicalFileHandler = new MedicalFileHandler();
        await MedicalFileHandler.GetMedicalFileAsync(userKey);
        OnPropertyChanged(nameof(MedicalFile)); // Notify the UI that MedicalFile has changed
    }

    private void OnMedicalFileReceived(MedicalFile medicalFile)
    {
        MedicalFile = medicalFile;
        OnPropertyChanged(nameof(MedicalFile)); // Notify the UI that the MedicalFile has been updated
    }
}