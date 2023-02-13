
namespace app.Entities
{
    /* public record User(
        Guid Id,
        String Name,
        String Surname,
        String Username,
        String Email,
        Role Role
        ) : IEntity
    {
        public Password Password { get; set; } = default!;
        public RefreshToken RefreshToken { get; set; } = default!;
        public ResetToken ResetToken { get; set; } = default!;
    }; */

    public class User : IEntity
    {
        public User(Guid id, string name, string surname, string username, string email, Role role, Password password)
        {
            Id = id;
            Name = name;
            Surname = surname;
            Username = username;
            Email = email;
            Role = role;
            Password = password;
        }

        public Guid Id { get; init; }
        public String Name { get; set; } = default!;
        public String Surname { get; set; } = default!;
        public String Username { get; set; } = default!;
        public String Email { get; set; } = default!;
        public Role Role { get; set; } = default!;
        public Password Password { get; set; } = default!;
        public RefreshToken RefreshToken { get; set; } = default!;
        public ResetToken ResetToken { get; set; } = default!;
    }
}
