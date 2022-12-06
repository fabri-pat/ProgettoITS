namespace app.Entities
{
    public record RefreshToken(
        byte[] Token,
        byte[] Sst,
        DateTime CreatedDate,
        DateTime ExpirationDate
    );
}