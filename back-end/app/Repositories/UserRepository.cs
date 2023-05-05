
using app.Entities;
using MongoDB.Driver;
using MongoDbBaseRepository.MongoDB;

namespace app.Repositories
{
    public class UserRepository : MongoRepository<User, Guid>, IUserRepository
    {
        public UserRepository(IServiceProvider serviceProvider) 
            : base(serviceProvider.GetService<IMongoDatabase>()!, typeof(User).Name)
        {}   
    }
}