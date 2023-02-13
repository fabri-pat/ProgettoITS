namespace app.Entities
{
    public record ResetToken(
        String Token,
        DateTime ExpireDate
    );
}