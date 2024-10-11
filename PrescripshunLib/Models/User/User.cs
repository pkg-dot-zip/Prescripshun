namespace PrescripshunLib.Models.User;

public class User
{
    public Guid UserKey { get; set; }
    public required string UserName { get; set; }
    public required string Password { get; set; }
    public Profile.Profile Profile { get; set; }

    public Guid? DoctоrGuid { get; set; } = null;
    public List<Guid> Patients { get; set; } = new();
    
    public bool IsDoctor => DoctоrGuid == Guid.Empty || DoctоrGuid is null;
}