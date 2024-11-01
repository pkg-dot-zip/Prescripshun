using PrescripshunGui.ViewModels;
using PrescripshunLib.Models.MedicalFile;
using PrescripshunLib.Models.User.Profile;
using System.Threading.Tasks;
using System;
using PrescripshunClient;
using PrescripshunLib.Networking.Messages;
using PrescripshunGui.Util;
using System.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;

public class ProfileViewModel : ViewModelBase
{
    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

    public Profile Profile { get; }
    private MedicalFile _medicalFile;
    public MedicalFile MedicalFile
    {
        get => _medicalFile;
        private set
        {
            _medicalFile = value;
            OnPropertyChanged(nameof(MedicalFile));
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public ProfileViewModel(Profile profile, Guid userkey)
    {
        Profile = profile;
        GuiEvents.Get.RegisterMedicalFileCallback(OnMedicalFileReceived);
        GetMedicalFileAsync(userkey);
        Logger.Info("ProfileViewModel initialized for user: {0}", userkey);
    }

    public ICommand MedicalFileCommand { get; private set; }

    public async Task GetMedicalFileAsync(Guid userKey)
    {
        MedicalFileCommand = new AsyncRelayCommand(async () =>
        {
            Logger.Info("Sending GetMedicalFileRequest for user: {0}", userKey);
            await NetworkHandler.Send(new GetMedicalFileRequest()
            {
                UserKey = userKey,
            });
        });

        MedicalFileCommand.Execute(null);
    }

    private void OnMedicalFileReceived(MedicalFile medicalFile)
    {
        Logger.Info("Medical file received for patient: {0}", medicalFile.Patient);
        MedicalFile = medicalFile;
    }

    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}