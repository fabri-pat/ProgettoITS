
namespace app.Entities
{
    public interface IEntity<K>
    {
        K Id { get; init; }
    }
}
