
using app.Entities;

public interface IPasswordService
{
    Password CreatePassword(String password);
    bool VerifyPassword(String passwordInserted, Password password);
}