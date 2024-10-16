using PrescripshunLib.Models.Chat;
using PrescripshunLib.Models.MedicalFile;
using PrescripshunLib.Models.User;

namespace Prescripshun.Database;

internal interface IDatabaseHandler
{
    /// <summary>
    /// Boots the Database.
    /// </summary>
    /// <returns></returns>
    public Task Run();

    /// <summary>
    /// Stops the Database. No requests can be made after this method has been called.
    /// </summary>
    /// <returns></returns>
    public Task Stop();

    /// <summary>
    /// Returns a list of users a user with guid <paramref name="forUser"/> can chat with. <br/>
    ///
    /// For a doctor this is his patients. <br/>
    /// For a patient this is ONLY his doctor. <br/>
    /// </summary>
    /// <param name="forUser"></param>
    /// <returns></returns>
    public List<User> GetChattableUsers(Guid forUser);

    /// <summary>
    /// Retrieves all users found in the database.
    /// </summary>
    /// <returns></returns>
    public List<User> GetUsers();

    /// <summary>
    /// Adds a doctor to the database.
    /// </summary>
    /// <param name="doctor"></param>
    /// <returns></returns>
    public Task AddDoctor(User doctor); // TODO: Merge with AddPatient.

    /// <summary>
    /// Retrieves all doctors found in the database.
    /// </summary>
    /// <returns></returns>
    public List<User> GetDoctors();

    /// <summary>
    /// Adds a patient to the database.
    /// </summary>
    /// <param name="patient"></param>
    /// <returns></returns>
    public Task AddPatient(User patient); // TODO: Merge with AddDoctor.

    /// <summary>
    /// Retrieves all patients found in the database.
    /// </summary>
    /// <returns></returns>
    public List<User> GetPatients();

    /// <summary>
    /// Retrieves this user by his guid.
    /// </summary>
    /// <param name="guid"></param>
    /// <returns></returns>
    public User GetUser(Guid guid);

    /// <summary>
    /// Inserts a <seealso cref="MedicalFile"/> into the database.
    /// </summary>
    /// <param name="medicalFile"></param>
    /// <returns></returns>
    public Task AddMedicalFile(MedicalFile medicalFile);

    /// <summary>
    /// Retrieves a medical file for a user with <paramref name="guid"/>.
    /// </summary>
    /// <param name="guid"></param>
    /// <returns></returns>
    public MedicalFile GetMedicalFile(Guid guid);

    /// <summary>
    /// Adds a chat instance to the database.
    /// </summary>
    /// <param name="chat"></param>
    /// <returns></returns>
    public Task AddChat(Chat chat);

    /// <summary>
    /// Retrieves a chat instance from the database between two given users.
    /// </summary>
    /// <param name="user1">guid of one user.</param>
    /// <param name="user2">guid of the other user.</param>
    /// <returns></returns>
    public Chat GetChat(Guid user1, Guid user2);

    /// <summary>
    /// Checks whether the <paramref name="username"/> and <paramref name="password"/> match a record.
    /// </summary>
    /// <param name="username">Username used for login attempt.</param>
    /// <param name="password">Password used for login attempt.</param>
    /// <param name="userKey">Guid of user that logged in, if applicable.</param>
    /// <param name="reason">Reason why logging in failed, if applicable.</param>
    /// <returns><c>True</c> if successful. <c>false</c> if failed.</returns>
    public bool TryLogin(string username, string password, out Guid userKey, out string reason);
}