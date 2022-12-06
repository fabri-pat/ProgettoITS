
namespace app.Entities
{
    public record User(
        Guid Id,
        String Name,
        String Surname,
        String Username,
        Password Password,
        String Role
    )
    {
        public RefreshToken RefreshToken { get; set; } = default!;
    };
}
