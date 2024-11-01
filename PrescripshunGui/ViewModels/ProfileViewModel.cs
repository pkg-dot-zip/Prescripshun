using System;
using System.ComponentModel;
using System.Windows.Input;
using PrescripshunLib.Models.MedicalFile;
using PrescripshunLib.Models.User.Profile;
using NLog;
using PrescripshunGui.Util;
using PrescripshunGui.ViewModels;
using PrescripshunLib.Networking.Messages;

public class ProfileViewModel : ViewModelBase
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public Profile Profile { get; }

    private MedicalFile _medicalFile;
    public MedicalFile MedicalFile
    {
        get => _medicalFile;
        private set
        {
            if (_medicalFile != value)
            {
                _medicalFile = value;
                OnPropertyChanged(nameof(MedicalFile));
            }
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public ProfileViewModel(Profile profile, Guid userKey)
    {
        Profile = profile;
        GuiEvents.Get.RegisterMedicalFileCallback(OnMedicalFileReceived);
        GetMedicalFile(userKey);
    }

    public void GetMedicalFile(Guid userKey)
    {
        Logger.Info("Sending GetMedicalFileRequest for user: {0}", userKey);
        NetworkHandler.Send(new GetMedicalFileRequest() { UserKey = userKey });
    }

    private void OnMedicalFileReceived(MedicalFile medicalFile)
    {
        MedicalFile = medicalFile;
    }
}
