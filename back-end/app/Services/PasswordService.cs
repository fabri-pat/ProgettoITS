using System.Security.Cryptography;
using app.Entities;

namespace app.Services
{
    public class PasswordService : IPasswordService
    {
        public Password CreatePassword(String password)
        {
            var hmac = new HMACSHA512();

            var passwordSalt = hmac.Key;
            var passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

            return new Password(passwordHash, passwordSalt);
        }

        public bool VerifyPassword(String passwordInserted, Password password)
        {
            var hmac = new HMACSHA512(password.PasswordSalt);

            var passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(passwordInserted));

            return passwordHash.SequenceEqual(password.PasswordHash);
        }
    }
}