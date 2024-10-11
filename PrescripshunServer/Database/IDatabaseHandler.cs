using PrescripshunLib.Models.Chat;
using PrescripshunLib.Models.MedicalFile;
using PrescripshunLib.Models.User;

namespace Prescripshun.Database;

internal interface IDatabaseHandler
{
    public Task Run();
    public Task Stop();

    public List<User> GetChattableUsers(Guid forUser);
    public List<User> GetUsers();
    public Task AddDoctor(User doctor); // TODO: Merge with AddPatient.
    public List<User> GetDoctors();
    public Task AddPatient(User patient); // TODO: Merge with AddDoctor.
    public List<User> GetPatients();
    public User GetUser(Guid guid);
    public Task AddMedicalFile(MedicalFile medicalFile);
    public MedicalFile GetMedicalFile(Guid guid);
    public Task AddChat(Chat chat);
    public Chat GetChat(Guid user1, Guid user2);

    public bool TryLogin(string username, string password, out Guid userKey, out string reason);
}