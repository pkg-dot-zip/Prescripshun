using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using PrescripshunGui.Util;
using PrescripshunLib.Models.MedicalFile;
using PrescripshunLib.Models.User.Profile;
using PrescripshunLib.Networking.Messages;
using Unclassified.Net;

namespace PrescripshunGui.ViewModels;

public class ProfileViewModel : ViewModelBase
{
    public Profile Profile { get; }
    public MedicalFile MedicalFile { get; set; }
    private AsyncTcpClient _client;

    public ProfileViewModel(Profile profile)
    {
        Profile = profile;
    }

    public void MedicalFileService(AsyncTcpClient client)
    {
        _client = client;
    }

    public async Task<MedicalFile> GetMedicalFileAsync(Guid userKey)
    {
        var request = new GetMedicalFileRequest { UserKey = userKey };
        await _client.Send(request) //ToDo omzetten naar bytes;

        var response = await _client //ToDo;
        if (response.MedicalFile != null)
        {
            return response.MedicalFile;
        }
        else
        {
            throw new Exception(response.Reason);
        }
    }
}
