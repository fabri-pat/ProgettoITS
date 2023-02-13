namespace app.Services
{
    public interface IEMailService
    {
        Task SendAsync(string to, string subject, string body);
    }
}