using PrescripshunLib.ExtensionMethods;
using PrescripshunLib.Models.MedicalFile;
using System.Net.Sockets;
using System.Text;
using Unclassified.Net;

namespace PrescripshunLib.Networking.Messages;

public class MedicalFileHandler
{
    private AsyncTcpClient TcpClient { get; set; }
    private Dictionary<Guid, TaskCompletionSource<MedicalFile>> pendingRequests = new();

    public async Task GetMedicalFileAsync(Guid userKey)
    {
        var tcs = new TaskCompletionSource<MedicalFile>();
        pendingRequests[userKey] = tcs;
        await TcpClient.Send(new GetMedicalFileRequest { UserKey = userKey });
    }
}