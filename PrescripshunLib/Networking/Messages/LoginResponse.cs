namespace PrescripshunLib.Networking.Messages;

public class LoginResponse : IMessage, IEquatable<LoginResponse>
{
    public Guid Id { get; set; } = Guid.Empty;
    public string Reason { get; set; } = string.Empty;

    public bool IsValid() => Id != Guid.Empty;

    public bool Equals(LoginResponse? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Id.Equals(other.Id) && Reason == other.Reason;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((LoginResponse) obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, Reason);
    }
}