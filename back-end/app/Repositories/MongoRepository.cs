
using System.Linq.Expressions;
using app.Entities;
using MongoDB.Driver;

namespace app.Repositories
{
    public abstract class MongoRepository<T> : IRepository<T> where T : IEntity
    {
        protected IMongoCollection<T> dbCollection;
        protected FilterDefinitionBuilder<T> filterDefinitionBuilder = Builders<T>.Filter;
        protected UpdateDefinitionBuilder<T> updateDefinitionBuilder = Builders<T>.Update;

        public MongoRepository(IMongoDatabase database, string collectionName)
        {
            dbCollection = database.GetCollection<T>(collectionName);
        }

        public virtual async Task<IReadOnlyCollection<T>> GetAllAsync()
        {
            return await dbCollection.Find(filterDefinitionBuilder.Empty).ToListAsync();
        }

        public virtual async Task<T> GetByIdAsync(Guid id)
        {
            FilterDefinition<T> filter = filterDefinitionBuilder.Eq(
                entity => entity.Id, id
            );

            return await dbCollection.Find(filter).FirstOrDefaultAsync();
        }

        public virtual async Task<T> GetByExpressionAsync(Expression<Func<T, bool>> filter)
        {
            return await dbCollection.Find(filter).FirstOrDefaultAsync();
        }

        public virtual async Task CreateAsync(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            await dbCollection.InsertOneAsync(entity);
            return;
        }

        public virtual async Task UpdateAsync(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            FilterDefinition<T> filter = filterDefinitionBuilder.Eq(x => x.Id, entity.Id);
            await dbCollection.ReplaceOneAsync(filter, entity);
        }

        public virtual async Task RemoveAsync(Guid id)
        {
            FilterDefinition<T> filter = filterDefinitionBuilder.Eq(x => x.Id, id);
            await dbCollection.DeleteOneAsync(filter);
        }
    }
}