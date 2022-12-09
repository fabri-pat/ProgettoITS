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

        /* public static User AsDao(this RegistrationRequestDto registrationRequestDto)
        {
            return new User(
                Id: new Guid(),
                Name: registrationRequestDto.Name,
                Surname: registrationRequestDto.Surname,
                Username: registrationRequestDto.Username,
                Password: PasswordService
            )
        } */
    }
}