using app.Entities;
using MongoDbBaseRepository;

namespace app.Repositories
{
    public interface IUserRepository : IRepository<User, Guid>
    {
    }
}
