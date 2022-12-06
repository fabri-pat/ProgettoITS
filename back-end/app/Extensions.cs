using app.Dtos;
using app.Entities;

namespace app
{
    public static class Extensions
    {
        public static UserDto AsDto(this User user)
        {
            return new UserDto(user.Id, user.Name, user.Surname, user.Username);
        }
    }
}