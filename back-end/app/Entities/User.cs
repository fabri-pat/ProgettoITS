
namespace app.Entities
{
    public record User(
        Guid Id,
        String Name,
        String Surname,
        String Username,
        String Email,
        Password Password,
        Role Role
        ) : IEntity
    {
        public RefreshToken RefreshToken { get; set; } = default!;
    };
}
