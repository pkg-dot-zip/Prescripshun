using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
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
        GetMedicalFileAsync(userKey);
        Logger.Info("ProfileViewModel initialized for user: {0}", userKey);
    }

    public ICommand MedicalFileCommand { get; private set; }

    private async Task GetMedicalFileAsync(Guid userKey)
    {
        MedicalFileCommand = new AsyncRelayCommand(async () =>
        {
            Logger.Info("Sending GetMedicalFileRequest for user: {0}", userKey);
            await NetworkHandler.Send(new GetMedicalFileRequest() { UserKey = userKey });
        });

        MedicalFileCommand.Execute(null);
    }

    private void OnMedicalFileReceived(MedicalFile medicalFile)
    {
        Logger.Info("Medical file received for patient: {0}", medicalFile.Patient);
        MedicalFile = medicalFile;
    }

    protected void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
