using Domain.Models;

public interface IAuthService
{
    bool Authenticate(string username, string password);
    string GetRole(string username);
}

public class AuthService : IAuthService
{
    private readonly List<User> _users = new List<User>
    {
        new User { Username = "admin", Password = "admin123", Role = "Admin" },
        new User { Username = "user", Password = "user123", Role = "User" }
    };

    public bool Authenticate(string username, string password)
    {
        return _users.Any(u => u.Username == username && u.Password == password);
    }

    public string GetRole(string username)
    {
        return _users.FirstOrDefault(u => u.Username == username)?.Role;
    }
}
