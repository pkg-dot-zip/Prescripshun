namespace PrescripshunLib.Networking.Messages;

public class LoginRequest : IMessage, IEquatable<LoginRequest>
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    public bool Equals(LoginRequest? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Username == other.Username && Password == other.Password;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((LoginRequest) obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Username, Password);
    }
}